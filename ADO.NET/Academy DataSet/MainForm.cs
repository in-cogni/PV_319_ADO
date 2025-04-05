using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel.Design;


namespace Academy_DataSet
{
    public partial class MainForm : Form
    {
        readonly string CONNECTION_STRING = "";
        SqlConnection connection;
        DataSet GroupsRelatedData;
        List<string> tables;
        public MainForm()
        {
            InitializeComponent();
            AllocConsole();
            CONNECTION_STRING = ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString;
            connection = new SqlConnection(CONNECTION_STRING);
            Console.WriteLine(CONNECTION_STRING);

            tables = new List<string>();
            GroupsRelatedData = new DataSet(nameof(GroupsRelatedData));
            //LoadGroupsRelatedData();
            Check();
        }
        public void AddTable(string table, string columns)
        {
            string[] separated_columns = columns.Split(',');
            GroupsRelatedData.Tables.Add(table);
            for (int i = 0; i < separated_columns.Length; i++)
            {
                GroupsRelatedData.Tables[table].Columns.Add(separated_columns[i]);
            }
            GroupsRelatedData.Tables[table].PrimaryKey = 
                new DataColumn[] {GroupsRelatedData.Tables[table].Columns[separated_columns[0]]};
            tables.Add($"{table},{columns}");
        }
        public void AddRelation(string name, string child, string parent)
        {
            GroupsRelatedData.Relations.Add
                (
                    name,
                    GroupsRelatedData.Tables[parent.Split(',')[0]].Columns[parent.Split(',')[1]],
                    GroupsRelatedData.Tables[child.Split(',')[0]].Columns[child.Split(',')[1]]
                );
        }
        public void Load()
        {
            string[] tables = this.tables.ToArray();
            for(int i = 0; i < tables.Length;i++)
            {
                string cmd = $"SELECT * FROM {tables[i].Split(',')[0]}";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd, connection);
                adapter.Fill(GroupsRelatedData.Tables[tables[i].Split(',')[0]]);
            }
        }
        void LoadGroupsRelatedData()
        {
            Console.WriteLine(nameof(GroupsRelatedData));
            //1) Создаем DataSet
            //Переносим в конструктор

            //2) Добавляем таблицы в DataSet

            const string dsTable_Directions = "Directions";
            const string dst_col_direction_id = "direction_id";
            const string dst_col_direcion_name = "direction_name";
            GroupsRelatedData.Tables.Add(dsTable_Directions);
            GroupsRelatedData.Tables[dsTable_Directions].Columns.Add(dst_col_direction_id, typeof(byte));
            GroupsRelatedData.Tables[dsTable_Directions].Columns.Add(dst_col_direcion_name, typeof(string));
            GroupsRelatedData.Tables[dsTable_Directions].PrimaryKey =
                new DataColumn[] { GroupsRelatedData.Tables[dsTable_Directions].Columns[dst_col_direction_id] };

#if true
            //var directionsTable = CreateDirectionsTable();
            //GroupsRelatedData.Tables.Add(directionsTable);  
#endif

            const string dsTable_Groups = "Groups";
            const string dst_Groups_col_group_id = "group_id";
            const string dst_Groups_col_group_name = "group_name";
            const string dst_Groups_col_direction = "direction";
            GroupsRelatedData.Tables.Add(dsTable_Groups);
            GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_Groups_col_group_id, typeof(int));
            GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_Groups_col_group_name, typeof(string));
            GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_Groups_col_direction, typeof(byte));
            GroupsRelatedData.Tables[dsTable_Groups].PrimaryKey =
                new DataColumn[] { GroupsRelatedData.Tables[dsTable_Groups].Columns[dst_Groups_col_group_id] };

#if true
            //var groupsTable = CreateGroupsTable();
            //GroupsRelatedData.Tables.Add(groupsTable);  
#endif

            //3) Строим связи между таблицами: (не меняла)

            string dsRelation_GroupsDirections = "GroupsDirection";
            GroupsRelatedData.Relations.Add
                (
                dsRelation_GroupsDirections, //Relation name
                GroupsRelatedData.Tables["Directions"].Columns["direction_id"], //Parent field (Primary key)
                GroupsRelatedData.Tables["Groups"].Columns["direction"] //Child field (Foreign key)
                );

            //4) Загружаем данные в таблицы:

            string direction_cmd = "SELECT * FROM Directions";
            string groups_cmd = "SELECT * FROM Groups";
            SqlDataAdapter directionsAdapter = new SqlDataAdapter(direction_cmd, connection);
            SqlDataAdapter groupsAdapter = new SqlDataAdapter(groups_cmd, connection);
            connection.Open();
            directionsAdapter.Fill(GroupsRelatedData.Tables[dsTable_Directions]);
            groupsAdapter.Fill(GroupsRelatedData.Tables[dsTable_Groups]);
            connection.Close();

#if true
            //const string directionsCmd = "SELECT * FROM Directions";
            //const string groupsCmd = "SELECT * FROM Groups";

            //using (var directionsAdapter = new SqlDataAdapter(directionsCmd, connection))
            //using (var groupsAdapter = new SqlDataAdapter(groupsCmd, connection))
            //{
            //    connection.Open();
            //    directionsAdapter.Fill(directionsTable);
            //    groupsAdapter.Fill(groupsTable);
            //    connection.Close();
            //}  
#endif

            //5) вывод:

            foreach (DataRow row in GroupsRelatedData.Tables[dsTable_Directions].Rows)
            {
                Console.WriteLine($"{row[dst_col_direction_id]}\t{row[dst_col_direcion_name]}");
            }
            Console.WriteLine("n----------------------------------------------\n");
            foreach (DataRow row in GroupsRelatedData.Tables[dsTable_Groups].Rows)
            {
                Console.WriteLine($"{row[dst_Groups_col_group_id]}\t{row[dst_Groups_col_group_name]}\t{row.GetParentRow(dsRelation_GroupsDirections)[dst_col_direcion_name]}");
            }

#if true
            //const string relationName = "GroupsDirection";

            //foreach (DataRow row in directionsTable.Rows)
            //{
            //    Console.WriteLine($"{row["direction_id"]}\t{row["direction_name"]}");
            //}
            //Console.WriteLine("\n----------------------------------------------\n");
            //foreach (DataRow row in groupsTable.Rows)
            //{
            //    var directionName = row.GetParentRow(relationName)["direction_name"];
            //    Console.WriteLine($"{row["group_id"]}\t{row["group_name"]}\t{directionName}");
            //}  
#endif
        }
        void Print(string table)
        {
            Console.WriteLine("----------------------------------------------------------------------------------------");
            foreach(DataRow row in GroupsRelatedData.Tables[table].Rows)
            {
                for(int i=0; i<row.ItemArray.Length; i++)
                {
                    Console.Write(row[i].ToString()+"\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine("----------------------------------------------------------------------------------------");
        }
        void Check()
        {
            AddTable("Directions", "direction_id,direction_name");
            AddTable("Groups", "group_id,group_name,direction");
            AddRelation("GroupsDirections", "Groups,direction", "Directions,direction_id");
            Load();
            Print("Directions");
            Print("Groups");
        }
#if true
        //DataTable CreateDirectionsTable()
        //{
        //    const string tableName = "Directions";
        //    const string idColumn = "direction_id";
        //    const string nameColumn = "direction_name";

        //    var table = new DataTable(tableName);
        //    table.Columns.Add(idColumn, typeof(byte));
        //    table.Columns.Add(nameColumn, typeof(string));
        //    table.PrimaryKey = new[] { table.Columns[idColumn] };

        //    return table;
        //}

        //DataTable CreateGroupsTable()
        //{
        //    const string tableName = "Groups";
        //    const string idColumn = "group_id";
        //    const string nameColumn = "group_name";
        //    const string directionColumn = "direction";

        //    var table = new DataTable(tableName);
        //    table.Columns.Add(idColumn, typeof(int));
        //    table.Columns.Add(nameColumn, typeof(string));
        //    table.Columns.Add(directionColumn, typeof(byte));
        //    table.PrimaryKey = new[] { table.Columns[idColumn] };

        //    return table;
        //}  
#endif
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
    }

}
