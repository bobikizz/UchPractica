using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class AdminForm : Form
    {
        // Динамический путь к базе данных
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inostr.accdb;";

        public AdminForm()
        {
            InitializeComponent();
            LoadWords(); // Загружаем список слов при запуске формы
        }

        // 1. ЗАГРУЗКА ДАННЫХ ИЗ БАЗЫ В ТАБЛИЦУ (Добавлен выбор ExampleSentence)
        private void LoadWords()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // Поменяли порядок: Категория теперь перед Примером
                string query = @"
            SELECT 
                W.WordID, 
                W.ForeighWord AS [Слово], 
                W.Translation AS [Перевод], 
                C.CategoryName AS [Категория],
                W.ExampleSentence AS [Пример в предложении]
            FROM (Words W
            LEFT JOIN WordCategories WC ON W.WordID = WC.WordID)
            LEFT JOIN Categories C ON WC.CategoryID = C.CategoryID";

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                DataTable dt = new DataTable();

                try
                {
                    adapter.Fill(dt);
                    dgvWords.DataSource = dt;

                    if (dgvWords.Columns["WordID"] != null)
                        dgvWords.Columns["WordID"].Visible = false;
                    // --- НАЧАЛО БЛОКА АВТОДОПОЛНЕНИЯ ---
                    // Создаем коллекцию для хранения подсказок
                    AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();

                    // Выбираем только уникальные названия категорий из загруженной таблицы dt
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Категория"] != DBNull.Value)
                        {
                            string catName = row["Категория"].ToString().Trim();
                            // Добавляем в список подсказок, если такого имени там еще нет
                            if (!autoCompleteCollection.Contains(catName) && !string.IsNullOrEmpty(catName))
                            {
                                autoCompleteCollection.Add(catName);
                            }
                        }
                    }

                    // Привязываем собранный список к нашему текстовому полю
                    txtCategory.AutoCompleteCustomSource = autoCompleteCollection;
                    // --- КОНЕЦ БЛОКА АВТОДОПОЛНЕНИЯ ---


                    // РЕШЕНИЕ: Растягиваем последний столбец, чтобы текст не обрезался
                    if (dgvWords.Columns["Пример в предложении"] != null)
                    {
                        dgvWords.Columns["Пример в предложении"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        // 2. КНОПКА ДОБАВИТЬ СЛОВО И СВЯЗАТЬ С КАТЕГОРИЕЙ
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 1. Проверяем, что главные поля заполнены
            if (string.IsNullOrWhiteSpace(txtWord.Text) || string.IsNullOrWhiteSpace(txtTranslation.Text) || string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все три поля: Слово, Перевод и Категорию!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int categoryId = 0;
            int wordId = 0;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // ==========================================
                    // ШАГ А: Работаем с категорией (Categories)
                    // ==========================================
                    string checkCategoryQuery = "SELECT CategoryID FROM Categories WHERE CategoryName = @catName";
                    OleDbCommand checkCatCmd = new OleDbCommand(checkCategoryQuery, connection);
                    checkCatCmd.Parameters.AddWithValue("@catName", txtCategory.Text.Trim());

                    object catIdResult = checkCatCmd.ExecuteScalar();

                    if (catIdResult != null)
                    {
                        categoryId = Convert.ToInt32(catIdResult);
                    }
                    else
                    {
                        string insertCatQuery = "INSERT INTO Categories (CategoryName) VALUES (@catName)";
                        OleDbCommand insertCatCmd = new OleDbCommand(insertCatQuery, connection);
                        insertCatCmd.Parameters.AddWithValue("@catName", txtCategory.Text.Trim());
                        insertCatCmd.ExecuteNonQuery();

                        insertCatCmd.CommandText = "SELECT @@IDENTITY";
                        categoryId = Convert.ToInt32(insertCatCmd.ExecuteScalar());
                    }

                    // ==========================================
                    // ШАГ Б: Добавляем слово в таблицу (Words) с подсказкой
                    // ==========================================
                    // ВАЖНО: Параметры для Access добавляются строго в том порядке, в котором они идут в SQL-запросе!
                    string insertWordQuery = "INSERT INTO Words (ForeighWord, [Translation], ExampleSentence, LanguageID, UserId) VALUES (@word, @trans, @example, @langId, @userId)";
                    OleDbCommand insertWordCmd = new OleDbCommand(insertWordQuery, connection);

                    insertWordCmd.Parameters.AddWithValue("@word", txtWord.Text.Trim());
                    insertWordCmd.Parameters.AddWithValue("@trans", txtTranslation.Text.Trim());
                    insertWordCmd.Parameters.AddWithValue("@example", txtExample.Text.Trim()); // Текст подсказки
                    insertWordCmd.Parameters.AddWithValue("@langId", 2);  // 2 — это English по умолчанию
                    insertWordCmd.Parameters.AddWithValue("@userId", 1);  // 1 — это ID администратора по умолчанию

                    insertWordCmd.ExecuteNonQuery();

                    // Получаем ID только что созданного слова
                    insertWordCmd.CommandText = "SELECT @@IDENTITY";
                    wordId = Convert.ToInt32(insertWordCmd.ExecuteScalar());

                    // ==========================================
                    // ШАГ В: Связываем слово и категорию (WordCategories)
                    // ==========================================
                    string linkQuery = "INSERT INTO WordCategories (WordID, CategoryID) VALUES (@wId, @cId)";
                    OleDbCommand linkCmd = new OleDbCommand(linkQuery, connection);
                    linkCmd.Parameters.AddWithValue("@wId", wordId);
                    linkCmd.Parameters.AddWithValue("@cId", categoryId);
                    linkCmd.ExecuteNonQuery();

                    MessageBox.Show("Слово, категория и пример успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Очищаем форму и обновляем таблицу на экране
                    ClearForm();
                    LoadWords();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении данных: " + ex.Message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 3. КНОПКА ОБНОВИТЬ БАЗУ
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // 1. Проверяем, что строка в таблице выбрана
            if (dgvWords.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите слово в таблице для редактирования!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Проверяем обязательные поля
            if (string.IsNullOrWhiteSpace(txtWord.Text) || string.IsNullOrWhiteSpace(txtTranslation.Text) || string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Поля Слово, Перевод и Категория не могут быть пустыми при обновлении!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем ID редактируемого слова из выделенной строки
            int wordId = Convert.ToInt32(dgvWords.SelectedRows[0].Cells["WordID"].Value);
            int categoryId = 0;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // ШАГ А: Проверяем/создаем категорию (как при добавлении)
                    string checkCategoryQuery = "SELECT CategoryID FROM Categories WHERE CategoryName = @catName";
                    OleDbCommand checkCatCmd = new OleDbCommand(checkCategoryQuery, connection);
                    checkCatCmd.Parameters.AddWithValue("@catName", txtCategory.Text.Trim());

                    object catIdResult = checkCatCmd.ExecuteScalar();

                    if (catIdResult != null)
                    {
                        categoryId = Convert.ToInt32(catIdResult);
                    }
                    else
                    {
                        string insertCatQuery = "INSERT INTO Categories (CategoryName) VALUES (@catName)";
                        OleDbCommand insertCatCmd = new OleDbCommand(insertCatQuery, connection);
                        insertCatCmd.Parameters.AddWithValue("@catName", txtCategory.Text.Trim());
                        insertCatCmd.ExecuteNonQuery();

                        insertCatCmd.CommandText = "SELECT @@IDENTITY";
                        categoryId = Convert.ToInt32(insertCatCmd.ExecuteScalar());
                    }

                    // ШАГ Б: Обновляем само слово в таблице Words (включая пример предложения)
                    // ВАЖНО: В Access OLEDB параметры добавляются строго в порядке их следования в запросе UPDATE!
                    string updateWordQuery = @"
                UPDATE Words 
                SET ForeighWord = @word, 
                    [Translation] = @trans, 
                    ExampleSentence = @example 
                WHERE WordID = @id";

                    OleDbCommand updateWordCmd = new OleDbCommand(updateWordQuery, connection);
                    updateWordCmd.Parameters.AddWithValue("@word", txtWord.Text.Trim());
                    updateWordCmd.Parameters.AddWithValue("@trans", txtTranslation.Text.Trim());
                    updateWordCmd.Parameters.AddWithValue("@example", txtExample.Text.Trim()); // Новый измененный пример
                    updateWordCmd.Parameters.AddWithValue("@id", wordId);

                    updateWordCmd.ExecuteNonQuery();

                    // ШАГ В: Обновляем привязку к категории в таблице WordCategories
                    // Сначала удаляем старую связь, затем добавляем актуальную
                    string deleteLinkQuery = "DELETE FROM WordCategories WHERE WordID = @wId";
                    OleDbCommand deleteLinkCmd = new OleDbCommand(deleteLinkQuery, connection);
                    deleteLinkCmd.Parameters.AddWithValue("@wId", wordId);
                    deleteLinkCmd.ExecuteNonQuery();

                    string insertLinkQuery = "INSERT INTO WordCategories (WordID, CategoryID) VALUES (@wId, @cId)";
                    OleDbCommand insertLinkCmd = new OleDbCommand(insertLinkQuery, connection);
                    insertLinkCmd.Parameters.AddWithValue("@wId", wordId);
                    insertLinkCmd.Parameters.AddWithValue("@cId", categoryId);
                    insertLinkCmd.ExecuteNonQuery();

                    MessageBox.Show("Изменения успешно сохранены в базе данных!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Очищаем форму и перезагружаем таблицу
                    ClearForm();
                    LoadWords();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении данных: " + ex.Message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        // Кнопка УДАЛИТЬ выделенное слово
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Проверяем, что админ выделил хотя бы одну строку в таблице
            if (dgvWords.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите строку в таблице для удаления!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Запрашиваем подтверждение удаления
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить выбранное слово?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                // Получаем системный ID выделенного слова
                int wordId = Convert.ToInt32(dgvWords.SelectedRows[0].Cells["WordID"].Value);

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // ШАГ 1: Сначала удаляем связи слова с категориями из таблицы WordCategories
                        string deleteLinkQuery = "DELETE FROM WordCategories WHERE WordID = @id";
                        OleDbCommand deleteLinkCmd = new OleDbCommand(deleteLinkQuery, connection);
                        deleteLinkCmd.Parameters.AddWithValue("@id", wordId);
                        deleteLinkCmd.ExecuteNonQuery();

                        // ШАГ 2: Теперь удаляем само слово из таблицы Words
                        string deleteWordQuery = "DELETE FROM Words WHERE WordID = @id";
                        OleDbCommand deleteWordCmd = new OleDbCommand(deleteWordQuery, connection);
                        deleteWordCmd.Parameters.AddWithValue("@id", wordId);
                        deleteWordCmd.ExecuteNonQuery();

                        MessageBox.Show("Слово успешно удалено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearForm();
                        LoadWords();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении слова: " + ex.Message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // 4. КЛИК ПО СТРОКЕ ТАБЛИЦЫ (Данные переносятся в поля, включая подсказку)
        private void dgvWords_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvWords.Rows[e.RowIndex];
                txtWord.Text = row.Cells["Слово"].Value.ToString();
                txtTranslation.Text = row.Cells["Перевод"].Value.ToString();
                txtCategory.Text = row.Cells["Категория"].Value != null ? row.Cells["Категория"].Value.ToString() : "";

                // РЕШЕНИЕ: Используем новое точное имя столбца из SELECT
                txtExample.Text = row.Cells["Пример в предложении"] != null ? row.Cells["Пример в предложении"].Value.ToString() : "";
            }
        }


        // Метод для быстрой очистки всех текстовых полей на форме
        private void ClearForm()
        {
            txtWord.Clear();
            txtTranslation.Clear();
            txtCategory.Clear();
            txtExample.Clear(); // Очищаем поле примера использования
        }
    }
}