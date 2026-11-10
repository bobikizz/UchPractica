using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class UserForm : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inostr.accdb;";
        private DataTable dtWords = new DataTable();
        private int currentWordIndex = 0;
        private int currentUserId;

        public UserForm(int userId)
        {
            InitializeComponent();
            this.currentUserId = userId;

            LoadCategoriesToComboBox(); // 1. Сначала загружаем категории в выпадающий список
            LoadWordsForTraining();     // 2. Затем загружаем слова (по умолчанию выберутся все)
        }


        // 1. ЗАГРУЗКА СЛОВ (Добавили поле W.ExampleSentence в запрос)
        // 1. ЗАГРУЗКА СЛОВ С ИХ КАТЕГОРИЯМИ ИЗ БАЗЫ ДАННЫХ
        private void LoadWordsForTraining()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // Базовый запрос
                string query = @"
            SELECT 
                W.ForeighWord, 
                W.Translation, 
                W.ExampleSentence, 
                C.CategoryName
            FROM (Words W
            LEFT JOIN WordCategories WC ON W.WordID = WC.WordID)
            LEFT JOIN Categories C ON WC.CategoryID = C.CategoryID";

                // Если выбрана конкретная категория (индекс не 0), добавляем фильтрацию
                if (cmbCategories.SelectedIndex > 0)
                {
                    query += " WHERE C.CategoryName = @catName";
                }

                OleDbCommand command = new OleDbCommand(query, connection);

                // Если фильтр нужен — передаем выбранное имя категории в параметр
                if (cmbCategories.SelectedIndex > 0)
                {
                    command.Parameters.AddWithValue("@catName", cmbCategories.SelectedItem.ToString());
                }

                OleDbDataAdapter adapter = new OleDbDataAdapter(command);

                try
                {
                    dtWords.Clear();
                    adapter.Fill(dtWords);

                    if (chkShuffle.Checked)
                    {
                        ShuffleDataTable(dtWords);
                    }

                    currentWordIndex = 0; // Сбрасываем индекс на начало при смене категории
                    ShowWord();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки слов для тренировки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        // 2. ОТОБРАЖЕНИЕ ТЕКУЩЕГО СЛОВА
        private void ShowWord()
        {
            if (dtWords.Rows.Count == 0)
            {
                lblForeignWord.Text = "В словаре пока нет слов!";
                if (lblCategory != null) lblCategory.Text = "";
                if (lblProgress != null) lblProgress.Text = "";
                txtUserAnswer.Enabled = false;
                btnCheck.Enabled = false;
                btnNext.Enabled = false;
                btnHelp.Enabled = false; // Отключаем созданную в дизайнере кнопку
                return;
            }

            if (currentWordIndex >= dtWords.Rows.Count)
            {
                lblForeignWord.Text = "🎉 Изучение завершено!";
                lblResult.Text = "Вы успешно проверили все доступные слова!";
                lblResult.ForeColor = Color.Green;

                if (lblCategory != null) lblCategory.Text = "";
                if (lblProgress != null) lblProgress.Text = "Прогресс: 100%";

                txtUserAnswer.Clear();
                txtUserAnswer.Enabled = false;
                btnCheck.Enabled = false;
                btnHelp.Enabled = false; // Отключаем кнопку в конце круга
                lblHelp.Text = "";

                btnNext.Enabled = true;
                btnNext.Text = "Повторить круг";
                btnNext.Focus();
                return;
            }

            btnNext.Text = "Следующее слово";
            txtUserAnswer.Enabled = true;
            btnHelp.Enabled = true; // Включаем кнопку для нового слова

            lblForeignWord.Text = dtWords.Rows[currentWordIndex]["ForeighWord"].ToString();

            if (lblCategory != null)
            {
                string category = dtWords.Rows[currentWordIndex]["CategoryName"].ToString();
                lblCategory.Text = !string.IsNullOrWhiteSpace(category) ? $"Тема: {category}" : "Тема: Общая";
            }

            if (lblProgress != null)
            {
                lblProgress.Text = $"Слово: {currentWordIndex + 1} из {dtWords.Rows.Count}";
            }

            txtUserAnswer.Clear();
            lblResult.Text = "";
            lblHelp.Text = ""; // Очищаем текст подсказки от прошлого слова!
            btnCheck.Enabled = true;
            btnNext.Enabled = false;
            txtUserAnswer.Focus();
        }

        // 3. ОБРАБОТЧИК НАЖАТИЯ НА КНОПКУ ПОДСКАЗКИ (Код для созданного в дизайнере метода)
        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                // Достаем пример предложения из текущей строки базы данных
                string example = dtWords.Rows[currentWordIndex]["ExampleSentence"].ToString().Trim();

                if (!string.IsNullOrWhiteSpace(example))
                {
                    lblHelp.Text = $"Пример: {example}";
                }
                else
                {
                    lblHelp.Text = "Для этого слова пример использования не задан.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выводе подсказки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. КНОПКА ПРОВЕРИТЬ ОТВЕТ
        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserAnswer.Text))
            {
                MessageBox.Show("Введите перевод слова!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string correctTranslation = dtWords.Rows[currentWordIndex]["Translation"].ToString().Trim();
                string userResponse = txtUserAnswer.Text.Trim();

                if (string.Equals(correctTranslation, userResponse, StringComparison.OrdinalIgnoreCase))
                {
                    lblResult.Text = "🎉 Правильно!";
                    lblResult.ForeColor = Color.Green;

                    btnCheck.Enabled = false;
                    btnHelp.Enabled = false; // Блокируем кнопку подсказки, так как ответ угадан
                    btnNext.Enabled = true;
                    btnNext.Focus();
                }
                else
                {
                    lblResult.Text = "❌ Неверно, попробуйте еще раз!";
                    lblResult.ForeColor = Color.Red;
                    txtUserAnswer.Focus();
                    txtUserAnswer.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке слова: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 5. КНОПКА СЛЕДУЮЩЕЕ СЛОВО
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentWordIndex >= dtWords.Rows.Count)
            {
                currentWordIndex = 0;
                dtWords.Clear();
                LoadWordsForTraining(); // Загрузит заново и перемешает, если галочка активна
                return;
            }

            currentWordIndex++;
            ShowWord();
        }


        private void chkShuffle_CheckedChanged(object sender, EventArgs e)
        {
            currentWordIndex = 0;       // Возвращаемся к первому слову
            dtWords.Clear();            // Сбрасываем текущую таблицу
            LoadWordsForTraining();     // Перезагружаем данные из базы с учетом галочки
        }

        // Метод случайного перемешивания строк в DataTable
        private void ShuffleDataTable(DataTable table)
        {
            Random rand = new Random();
            for (int i = table.Rows.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                // Меняемся значениями элементов строки
                object[] temp = table.Rows[i].ItemArray;
                table.Rows[i].ItemArray = table.Rows[j].ItemArray;
                table.Rows[j].ItemArray = temp;
            }
        }

        private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadWordsForTraining(); // Просто перезагружаем слова с новым фильтром
        }
        // Метод для заполнения выпадающего списка доступными категориями
        private void LoadCategoriesToComboBox()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // Отключаем на время обработчик событий, чтобы выбор не срабатывал при заполнении
                cmbCategories.SelectedIndexChanged -= cmbCategories_SelectedIndexChanged;

                string query = "SELECT CategoryName FROM Categories ORDER BY CategoryName ASC";
                OleDbCommand command = new OleDbCommand(query, connection);

                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();

                    cmbCategories.Items.Clear();
                    cmbCategories.Items.Add("Все категории"); // Делаем дефолтный пункт

                    while (reader.Read())
                    {
                        string catName = reader["CategoryName"].ToString().Trim();
                        if (!string.IsNullOrEmpty(catName))
                        {
                            cmbCategories.Items.Add(catName);
                        }
                    }

                    cmbCategories.SelectedIndex = 0; // Выбираем по умолчанию "Все категории"
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки категорий: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Возвращаем обработчик событий на место
                    cmbCategories.SelectedIndexChanged += cmbCategories_SelectedIndexChanged;
                }
            }
        }

    }
}
