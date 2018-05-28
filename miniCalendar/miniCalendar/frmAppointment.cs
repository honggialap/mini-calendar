﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace miniCalendar
{
    public partial class frmAppointment : UserControl
    {
        DataTable dataTable = new DataTable();
        Dictionary<DateTime, List<Appointment>> groupDateTasks = new Dictionary<DateTime, List<Appointment>>();
        List<List<Appointment>> taskOfDay = new List<List<Appointment>>();
        List<Button> taskContainer = new List<Button>();
        

        // thuộc tính
        int totalWidth = 380;


        public frmAppointment()
        {
            InitializeComponent();
        }

        public frmAppointment(DataTable dataTable)
        {
            InitializeComponent();
            this.dataTable = dataTable;

            
            groupTaskAccordingToDay();
            sortingTaskAccordingToTime();


            DrawTimeTable();
        }
        private void btnNewAppointment_Click(object sender, EventArgs e)
        {
            frmNewAppointment form = new frmNewAppointment(dataTable, monthCalendar.SelectionRange.Start, false);
            form.Dock = DockStyle.Fill;
            this.panelTimeTable.Controls.Add(form);
            form.BringToFront();
            
        }

        private void monthCalendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            updateDrawingTimeTable();

            DrawTimeTable();

        }

        // 
        // PROCESSING TO DRAW TASKS ON TIMETABLE
        //

        // Nhóm các appoinment vào dictionary với key là ngày bắt đầu
        public void groupTaskAccordingToDay()
        {
            foreach (Appointment i in dataTable.dataTable)
            {
                DateTime time = new DateTime(i.startHour.Year, i.startHour.Month, i.startHour.Day);
                if (groupDateTasks.ContainsKey(time) == false)
                {
                    groupDateTasks.Add(time, new List<Appointment>());
                }
                groupDateTasks[time].Add(i);
            }
        }

        // Sắp xếp tăng dần theo thời gian bắt đầu của tất cả appointment trong mỗi ngày
        public void sortingTaskAccordingToTime()
        {
            foreach (KeyValuePair<DateTime, List<Appointment>> i in groupDateTasks)
            {
                for (int a = 0; a < i.Value.Count(); a++)
                {
                    for (int b = a + 1; b < i.Value.Count(); b++)
                    {
                        if (i.Value[a].startHour > i.Value[b].startHour)
                        {
                            var temp = i.Value[a];
                            i.Value[a] = i.Value[b];
                            i.Value[b] = temp;
                        }
                    }
                }
            }

        }

        // Hàm có chức năng sắp các task trong một ngày theo từng CỘT 
        // giúp tránh việc đụng độ khi thể hiện vẽ task ra màn hình
        public void taskOfDay_Building(List<Appointment> list)
        {
            foreach (Appointment i in list)
            {
                if(taskOfDay.Count() == 0)
                {
                    taskOfDay.Add(new List<Appointment>());
                    taskOfDay[0].Add(i);
                } 
                else
                {
                    bool isDone = false;
                    foreach (List<Appointment> l in taskOfDay)
                    {
                        bool isFill = false;
                        foreach (Appointment a in l)
                        {
                            if ((a.startHour <= i.startHour && i.startHour < a.endHour) ||
                                (a.startHour < i.endHour && i.endHour <= a.endHour))
                            {
                                isFill = true;
                                break;
                            }
                        }
                        if (isFill == false)
                        {
                            l.Add(i);
                            isDone = true;
                            break;
                        }
                    }

                    if (isDone == false)
                    {
                        taskOfDay.Add(new List<Appointment>());
                        taskOfDay[taskOfDay.Count() - 1].Add(i);
                    }
                }
            }
        }

        // Hàm này được gọi khi có sự kiện Datechanged, thêm appointment...
        // clear tất cả các container cũ
        // tạo lại mới container
        public void updateDrawingTimeTable() 
        {
            // clear old data
            taskOfDay.Clear();
            taskContainer.Clear();
            groupDateTasks.Clear();


            // update new data
            groupTaskAccordingToDay();
            sortingTaskAccordingToTime();

     
        }

        // Hàm hiển thị tất cả các task của cùng một ngày ra màn hình
        public void DrawTimeTable()
        {
            // clear old task
            for (int i = panelTimeTable.Controls.Count - 1; i >= 0; i--)
            {
                if (panelTimeTable.Controls[i] is Button)
                {
                    panelTimeTable.Controls.RemoveAt(i);
                }
            }



            // lấy ngày hiện tại của lịch chọn
            DateTime currentTime = new DateTime(monthCalendar.SelectionRange.Start.Year,
                                                monthCalendar.SelectionRange.Start.Month,
                                                monthCalendar.SelectionRange.Start.Day);

            if (groupDateTasks.ContainsKey(currentTime) == false)
            {
                return;
            }

            List<Appointment> list = new List<Appointment>(groupDateTasks[currentTime]);

            taskOfDay_Building(list);

            int numsTasks = groupDateTasks[currentTime].Count();
            int numsColumn = taskOfDay.Count();
            
            if (numsColumn == 1)
            {
                numsColumn = 2;
            }

           
           // MessageBox.Show(Convert.ToString(numsColumn) + "\n" + Convert.ToString(numsTasks));

            int idCol = 1;
            foreach(List<Appointment> col in taskOfDay)
            {
               foreach(Appointment a in col)
               {
                    drawingTask(a, idCol, numsColumn);
               }

               ++idCol;
            }
        }

        // hàm vẽ task ra màn hình, sử dụng thuộc tích richtextbox
        public void drawingTask(Appointment a, int col, int numsCol)
        {
            // width
            float width = (float)totalWidth / numsCol;

            // height
            int totalMinutes = (int)(a.endHour - a.startHour).TotalMinutes;
            float height = ((float)totalMinutes / 60) * (float)22.15; // 23 là độ rộng của một giờ trong timeTable

            // location
            float X = width * (col - 1) + 41;
            float Y = (a.startHour.Hour * 2 + a.startHour.Minute / 30) * (float)11.075; // 11.5 pixel là độ dài mỗi khoảng nửa giờ trong timeTable

            // color
            Color color = new Color();
            switch (a.Color)
            {
                case "Red":
                    color = Color.FromArgb(192, 0, 0);
                    break;
                case "Orange":
                    color = Color.FromArgb(255, 128, 0);
                    break;
                case "Yellow":
                    color = Color.FromArgb(192, 192, 0);
                    break;
                case "Green":
                    color = Color.FromArgb(0, 192, 0);
                    break;
                case "Blue":
                    color = Color.FromArgb(0, 192, 192);
                    break;
            }

            // draw
            Button task = new Button();
            task.Location = new Point((int)X, (int)Y);
            task.Width = (int)width;
            task.Height = (int)height;
            task.Text = a.Title;
            task.BackColor = color;
            task.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            task.ForeColor = System.Drawing.Color.White;

            taskContainer.Add(task); // thêm vào list task để hồi xoá hết cho dễ <3
            panelTimeTable.Controls.Add(task); // thêm vào panel
            task.Visible = true;
            task.BringToFront();
        }
    }
}
