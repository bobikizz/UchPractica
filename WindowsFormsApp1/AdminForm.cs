using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class AdminForm : Form
    {
        // Динамический путь к базе данных (работает на любом компьютере)
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inostr.accdb;";

        public AdminForm()
        {
            InitializeComponent();
            LoadWords(); // Загружаем список слов при запуске формы
        }

        // 1. ЗАГРУЗКА ДАННЫХ ИЗ БАЗЫ В ТАБЛИЦУ (Вывод слов и их категорий)
        private void LoadWords()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // Запрос связывает слова из таблицы Words с их названиями категорий из таблицы Categories
                string query = @"
                    SELECT 
                        W.WordID, 
                        W.ForeighWord AS [Слово], 
                        W.Translation AS [Перевод], 
                        C.CategoryName AS [Категория]
                    FROM (Words W
                    LEFT JOIN WordCategories WC ON W.WordID = WC.WordID)
                    LEFT JOIN Categories C ON WC.CategoryID = C.CategoryID";

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                DataTable dt = new DataTable();

                try
                {
                    adapter.Fill(dt);
                    dgvWords.DataSource = dt;

                    // Прячем системный ID, чтобы таблица выглядела красиво
                    if (dgvWords.Columns["WordID"] != null)
                        dgvWords.Columns["WordID"].Visible = false;
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
                    // ШАГ Б: Добавляем слово в таблицу (Words)
                    // ==========================================
                    // ИСПРАВЛЕНО: Добавлено поле UserId для обеспечения целостности с таблицей 'Users'
                    string insertWordQuery = "INSERT INTO Words (ForeighWord, [Translation], LanguageID, UserId) VALUES (@word, @trans, @langId, @userId)";
                    OleDbCommand insertWordCmd = new OleDbCommand(insertWordQuery, connection);

                    insertWordCmd.Parameters.AddWithValue("@word", txtWord.Text.Trim());
                    insertWordCmd.Parameters.AddWithValue("@trans", txtTranslation.Text.Trim());
                    insertWordCmd.Parameters.AddWithValue("@langId", 2);  // 2 — это English по умолчанию из твоей базы
                    insertWordCmd.Parameters.AddWithValue("@userId", 1);  // 1 — это ID администратора по умолчанию. В твоей базе админ как раз идет под первым ID

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

                    MessageBox.Show("Слово и категория успешно сохранены без ошибок целостности!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Очищаем форму и обновляем таблицу на экране
                    txtWord.Clear();
                    txtTranslation.Clear();
                    txtCategory.Clear();
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
            LoadWords();
            txtWord.Clear();
            txtTranslation.Clear();
            txtCategory.Clear();
            MessageBox.Show("Список слов обновлен!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            // Запрашиваем подтверждение удаления, чтобы админ не стёр слово случайно
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

                        // Очищаем поля ввода и обновляем таблицу на экране
                        txtWord.Clear();
                        txtTranslation.Clear();
                        txtCategory.Clear();
                        LoadWords();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении слова: " + ex.Message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        // 4. КЛИК ПО СТРОКЕ ТАБЛИЦЫ (Данные переносятся в поля для удобства)
        private void dgvWords_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvWords.Rows[e.RowIndex];
                txtWord.Text = row.Cells["Слово"].Value.ToString();
                txtTranslation.Text = row.Cells["Перевод"].Value.ToString();
                txtCategory.Text = row.Cells["Категория"].Value != null ? row.Cells["Категория"].Value.ToString() : "";
            }
        }
    }
}
