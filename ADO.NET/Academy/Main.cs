using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;

namespace Academy
{
    public partial class Main : Form
    {
        Connector connector;
        Dictionary<string, int> d_directions;
        Dictionary<string, int> d_groups;
        DataGridView[] tables;

        Query[] queries = new Query[]
        {
            new Query
            (
                "last_name,first_name,middle_name,birth_date,group_name,direction_name",
                            "Students JOIN Groups ON ([group]=group_id) JOIN Directions ON (direction=direction_id)"
                            //"[group]=group_id AND direction=direction_id"
                   ),
            new Query
            (
                "group_name,dbo.GetLearningDaysFor(group_name) AS weekdays,start_time,direction_name",
                            "Groups,Directions",
                            "direction=direction_id"

                ),
            new Query
            (
                "direction_name,COUNT(DISTINCT group_id) AS N'Количество групп', COUNT(stud_id) AS N'Количество студентов'",
                       "Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
                       "",
                       "direction_name"

                ),
            new Query("*", "Disciplines"),
             new Query("*", "Teachers")
        };
        string[] status_messages = new string[]
        {
            $"Количество студентов",
            $"Количество групп",
            $"Количество направлений",
            $"Количество дисциплин",
            $"Количество преподавателей"
        };
        public Main()
        {
            InitializeComponent();
            tables = new DataGridView[]
            {
                dgvStudents,
                dgvGroups,
                dgvDirections,
                dgvDisciplines,
                dgvTeachers
            };

            connector = new Connector
                (
                    ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString, toolStripStatusLabelCount
                );
            d_directions = connector.GetDictionary("*", "Directions");
            d_groups = connector.GetDictionary("group_id,group_name", "Groups");
            
            cbStudentsGroups.Items.AddRange(d_groups.Select(g => g.Key).ToArray());
            cbGroupsDirections.Items.AddRange(d_directions.Select(k => k.Key).ToArray());
            cbStudentsDirection.Items.AddRange(d_directions.Select(d => d.Key).ToArray());
            cbStudentsGroups.Items.Insert(0, "Все группы");
            cbStudentsDirection.Items.Insert(0, "Все направления");
            cbStudentsDirection.SelectedIndex = cbStudentsGroups.SelectedIndex = 0;
            //dgv - DataGridView
            dgvStudents.DataSource = connector.Select
                (
                    "last_name,first_name,middle_name,birth_date,group_name,direction_name",
                    "Students,Groups,Directions",
                     "[group]=group_id AND direction=direction_id"
                );
            //dgvGroups.DataSource = connector.Select("*", "Groups");
            //dgvDirections.DataSource = connector.Select("*", "Directions");
            //dgvDisciplines.DataSource = connector.Select("*", "Disciplines");
            //dgvTeachers.DataSource = connector.Select("*", "Teachers");
            //connector.StatusStrip();

            toolStripStatusLabelCount.Text = $"Количество студентов: {dgvStudents.RowCount - 1}.";
            loadPage(0);
            connector.ComboBox(comboBoxGroups, "Directions", "direction_name");
            connector.ComboBox(comboBoxStDirections, "Directions", "direction_name");
        }
        void loadPage(int i, Query query = null)
        {
            if(query==null)query = queries[tabControl.SelectedIndex];
            tables[i].DataSource
                = connector.Select(query.Columns, query.Tables, query.Condition, query.Group_by);
            toolStripStatusLabelCount.Text = status_messages[i] + CountRecordInDGV(tables[i]);

        }
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int i = tabControl.SelectedIndex;
            loadPage(tabControl.SelectedIndex);
#if true
            //switch(tabControl.SelectedIndex)
            //{
            //    case 0:
            //        dgvStudents.DataSource = 
            //            connector.Select
            //            (
            //                "last_name,first_name,middle_name,birth_date,group_name,direction_name", 
            //                "Students,Groups,Directions",
            //                "[group]=group_id AND direction=direction_id"
            //            );
            //        toolStripStatusLabelCount.Text = $"Количество студентов: {dgvStudents.RowCount - 1}.";
            //        comboBoxStDirections.Text = "Отобразить по направлению";
            //        comboBoxStGroups.Text = "Группа";
            //        break;
            //    case 1:
            //        dgvGroups.DataSource = connector.Select
            //            (
            //                "group_name,dbo.GetLearningDaysFor(group_name) AS weekdays,start_time,direction_name",
            //                "Groups,Directions",
            //                "direction=direction_id"
            //            );
            //        toolStripStatusLabelCount.Text = $"Количество групп: {dgvGroups.RowCount - 1}.";
            //        comboBoxGroups.Text = "Отобразить по направлению";
            //        break;
            //    case 2:
            //        //dgvDirections.DataSource = connector.Select
            //        //    (
            //        //    //     "d.direction_id,d.direction_name," +
            //        //    //     "(SELECT COUNT(*) FROM Groups g WHERE g.direction = d.direction_id)" +
            //        //    //     " AS number_of_group," +
            //        //    //     "(SELECT COUNT(*) FROM Students s WHERE s.[group] IN (SELECT group_id FROM Groups WHERE direction = d.direction_id))" +
            //        //    //     " AS number_of_student",
            //        //    //     "Directions d"
            //        //    "direction_name,COUNT(DISTINCT group_id) AS N'Количество групп', COUNT(stud_id) AS N'Количество студентов'",
            //        //    "Students,Groups,Directions",
            //        //    "[group]=group_id AND direction=direction_id",
            //        //    "direction_name"
            //        //    );
            //        dgvDirections.DataSource = connector.Select
            //           (
            //           "direction_name,COUNT(DISTINCT group_id) AS N'Количество групп', COUNT(stud_id) AS N'Количество студентов'",
            //           //"Groups JOIN Directions ON (direction=direction_id) JOIN Students ON ([group]=group_id)",
            //           "Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
            //           "",
            //           "direction_name"
            //           );
            //        toolStripStatusLabelCount.Text = $"Количество направлений: {dgvDirections.RowCount - 1}.";
            //        break;
            //    case 3:
            //        dgvDisciplines.DataSource = connector.Select("*", "Disciplines");
            //        toolStripStatusLabelCount.Text = $"Количество дисциплин: {dgvDisciplines.RowCount - 1}.";
            //        break;
            //    case 4:
            //        dgvTeachers.DataSource = connector.Select("*", "Teachers");
            //        toolStripStatusLabelCount.Text = $"Количество преподавателей: {dgvTeachers.RowCount - 1}.";
            //        break;
            //}  
#endif
        }

        private void comboBoxGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            string directionNameCombo = comboBoxGroups.SelectedItem.ToString();//название выбранного направления
            dgvGroups.DataSource = connector.Select
                (
                    "group_name,dbo.GetLearningDaysFor(group_name) AS weekdays,start_time,direction_name",
                    "Groups,Directions",
                    $"direction=direction_id AND direction_name=N'{directionNameCombo}'"
                );
        }

        private void comboBoxStGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            string directionNameCombo = comboBoxStDirections.SelectedItem.ToString();//название выбранного направления 
            string groupNameCombo = comboBoxStGroups.SelectedItem.ToString();//название выбранной группы
            dgvStudents.DataSource = connector.Select
                (
                    "last_name,first_name,middle_name,birth_date,group_name,direction_name",
                    "Students,Groups,Directions",
                    $"[group]=group_id AND direction=direction_id AND direction_name=N'{directionNameCombo}' AND group_name=N'{groupNameCombo}'"
                );
        }

        private void comboBoxStDirections_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxStGroups.Text = "Группа";
            comboBoxStGroups.Items.Clear();
            string directionNameCombo = comboBoxStDirections.SelectedItem.ToString();//название выбранного направления
            dgvStudents.DataSource = connector.Select
                 (
                     "last_name,first_name,middle_name,birth_date,group_name,direction_name",
                     "Students,Groups,Directions",
                     $"[group]=group_id AND direction=direction_id AND direction_name=N'{directionNameCombo}'"
                 );
            connector.ComboBox
                (
                comboBoxStGroups, "Groups,Directions", "group_name",
                $"direction=direction_id AND direction_name=N'{directionNameCombo}'"
                );
        }

        private void cbGroupsDirections_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvGroups.DataSource = connector.Select
                (
                "group_name,dbo.GetLearningDaysFor(group_name) AS weekdays,start_time,direction_name",
                "Groups,Directions",
                $"direction=direction_id AND direction = N'{d_directions[cbGroupsDirections.SelectedItem.ToString()]}'"
                );
            toolStripStatusLabelCount.Text = $"Количество групп : {CountRecordInDGV(dgvGroups)}";
        }
        int CountRecordInDGV(DataGridView dgv)
        {
            return dgv.RowCount == 0 ? 0 : dgv.Rows.Count - 1;
        }

        private void checkBEmptyDirections_CheckedChanged(object sender, EventArgs e)//пустые направления в Directions
        {
            checkBUnEmptyDirections.Checked = true ? false : true;//убираем галку во втором checkBox, чтобы 2 галки не стояли одновременно
            dgvDirections.Columns.Clear();
            if (checkBEmptyDirections.Checked)//если нажали галку
            {
                dgvDirections.DataSource = connector.Select
                    (
                        "direction_name,COUNT(DISTINCT group_id) AS N'Количество групп', COUNT(stud_id) AS N'Количество студентов'",
                        "Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
                        "",
                        "direction_name HAVING COUNT(DISTINCT group_id)=0 AND COUNT(stud_id)=0"//тут добавлено условие отбора 
                    );                                                                           //если есть пустые строки
            }
            else//если сняли галку - возвращаем все обратно
            {
                dgvDirections.DataSource = connector.Select
                    (
                        "direction_name,COUNT(DISTINCT group_id) AS N'Количество групп', COUNT(stud_id) AS N'Количество студентов'",
                        "Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
                        "",
                        "direction_name"
                    );
            }
        }

        private void checkBUnEmptyDirections_CheckedChanged(object sender, EventArgs e)//частично или полностью заполненные направления в Directions
        {
            checkBEmptyDirections.Checked = true ? false : true;//убираем галку во втором checkBox
            dgvDirections.Columns.Clear();
            if (checkBUnEmptyDirections.Checked)//если нажали галку
            {
                dgvDirections.DataSource = connector.Select
                    (
                        "direction_name,COUNT(DISTINCT group_id) AS N'Количество групп', COUNT(stud_id) AS N'Количество студентов'",
                        "Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
                        "",
                        "direction_name HAVING COUNT(DISTINCT group_id)!=0 OR COUNT(stud_id)!=0"//тут добавлено условие отбора 
                    );                                                                           //если хотя бы 1 строка не пустая
            }
            else//если сняли галку - возвращаем все обратно
            {
                dgvDirections.DataSource = connector.Select
                    (
                        "direction_name,COUNT(DISTINCT group_id) AS N'Количество групп', COUNT(stud_id) AS N'Количество студентов'",
                        "Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
                        "",
                        "direction_name"
                    );
            }
        }

        private void cbStudentsDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = cbStudentsDirection.SelectedIndex;
            
            Dictionary<string, int> d_groups = connector.GetDictionary
                (
                "group_id,group_name", 
                "Groups", 
                i==0?"":$"direction={d_directions[cbStudentsDirection.SelectedItem.ToString()]}"
                );
            cbStudentsGroups.Items.Clear();
            cbStudentsGroups.Items.AddRange(d_groups.Select(g=>g.Key).ToArray());

            //int t = tabControl.SelectedIndex;
            //dgvStudents.DataSource = 
            //    connector.Select
            //    (
            //    queries[t].Columns, 
            //    queries[t].Tables,
            //    i==0?"":$"direction={d_directions[cbStudentsDirection.SelectedItem.ToString()]}"

            //    );
            Query query = new Query(queries[0]);
            query.Condition = (i == 0 ? "" : $"direction={d_directions[cbStudentsDirection.SelectedItem.ToString()]}");
            loadPage(0, query);
        }

       
    }
}
