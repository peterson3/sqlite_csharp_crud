using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace CSHARP_SQLITE_CRUD
{
    public partial class Form1 : Form
    {
        private static string conexao = "Data Source=Banco.db";
        private static string nomebanco = "Banco.db";
        private static int IDRegistro;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(nomebanco))
            {
                SQLiteConnection.CreateFile(nomebanco);
                SQLiteConnection conn = new SQLiteConnection(conexao);
                conn.Open();

                StringBuilder sql_query = new StringBuilder();
                sql_query.AppendLine("CREATE TABLE IF NOT EXISTS PESSOAS ([ID] INTEGER PRIMARY KEY AUTOINCREMENT,");
                sql_query.AppendLine("[NOME] VARCHAR(50))");

                SQLiteCommand sql_cmd = new SQLiteCommand(sql_query.ToString(), conn);
                try
                {
                    sql_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro Ao Criar Banco de Dados: " + ex.Message);
                }
            }
            Carregar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand sql_cmd = new SQLiteCommand("INSERT INTO PESSOAS (NOME) VALUES (@NOME)", conn);
            sql_cmd.Parameters.AddWithValue("NOME", textBox1.Text.Trim());

            try
            {
                sql_cmd.ExecuteNonQuery();
                MessageBox.Show("Registro Salvo");
                textBox1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar registro: " + ex.Message);
            }

            Carregar();
        }

        private void Carregar()
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand sql_cmd = new SQLiteCommand("SELECT * FROM PESSOAS", conn);
            SQLiteDataReader sql_dataReader = sql_cmd.ExecuteReader();
            List<Pessoa> pessoas = new List<Pessoa>();
            while (sql_dataReader.Read())
            {
                pessoas.Add(new Pessoa
                {
                    Id = Convert.ToInt32(sql_dataReader["Id"]),
                    Nome = sql_dataReader["Nome"].ToString()
                });
            }
            dataGridView1.DataSource = pessoas;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand sql_cmd = new SQLiteCommand("DELETE FROM PESSOAS WHERE ID = @CODIGO", conn);
            sql_cmd.Parameters.AddWithValue("CODIGO", Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
            try
            {
                sql_cmd.ExecuteNonQuery();
                MessageBox.Show("Registro Excluido");
                Carregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao Tentar Excluir: " + ex.Message);
            }

        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            IDRegistro = 0;
            IDRegistro = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            textBox1.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (IDRegistro > 0)
            {
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();


                SQLiteCommand sql_cmd = new SQLiteCommand("UPDATE PESSOAS SET NOME = @NOME WHERE ID = @CODIGO", conn);
                sql_cmd.Parameters.AddWithValue("CODIGO", IDRegistro);
                sql_cmd.Parameters.AddWithValue("NOME", textBox1.Text);
                try
                {
                    sql_cmd.ExecuteNonQuery();
                    MessageBox.Show("Registro Atualizado");
                    Carregar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao Tentar Atualizar: " + ex.Message);
                }
            }
        }
    }
}
