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
    public partial class frmTaskDetail : UserControl
    {
        public int _id;
        Dictionary<int, Task> _todoList = new Dictionary<int, Task>();
        public bool _isModified;

        public DateTime _dueDay;
        public Point _remindTime;
        public DateTime _remindDay;
        public string _note = "";
        public string _comment = ""; 

        public frmTaskDetail(int id, Dictionary<int, Task> todoList, bool isModified) 
        {
            InitializeComponent();

            _id = id;
            _todoList = todoList;
            _isModified = isModified;
            tbSubtask.text = "Add a to-do...";

            ShowInfo(_isModified);
        }

        private void tbSubtask_KeyDown(object sender, EventArgs e)
        {
            if (tbSubtask.text == "Add a to-do...")
            {
                tbSubtask.text = "";
            }
        }

        private void tbSubtask_Enter(object sender, EventArgs e)
        {
            if (tbSubtask.text == "Add a to-do...")
            {
                tbSubtask.BackColor = Color.White;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(dtpRemindDay.Value > dtpDueDay.Value)
            {
                MessageBox.Show("Change your 'Remind Day' before clicking 'Save' button!", "Error!!!");
            }
            else
            {
                _isModified = true;
                ShowInfo(_isModified);
            }
            lbInfo.Text = "Last updated on " + DateTime.Now;
        }

        private void ShowInfo(bool isModified)
        {
            if (isModified == false)
            {
                tbTaskName.Text = _todoList[_id].Name;  
                dtpDueDay.Value = _todoList[_id].DueDay;
                dtpRemindTime.Value = _todoList[_id].RemindTime;
                dtpRemindDay.Value = _todoList[_id].RemindDay;
                rtbNote.Text = _todoList[_id].Note;
                lbInfo.Text = "Created on " + _todoList[_id].StartDay.ToString();
            }
            else
            {
                _todoList[_id].Name = tbTaskName.Text;
                _todoList[_id].DueDay = dtpDueDay.Value;
                _todoList[_id].RemindTime = dtpRemindTime.Value;
                _todoList[_id].RemindDay = dtpRemindDay.Value;
                _todoList[_id].Note = rtbNote.Text;
                _todoList[_id].StartDay = DateTime.Now;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _isModified = false;
            ShowInfo(_isModified);
        }

        private void cbIsDone_OnChange(object sender, EventArgs e)
        {
            if (cbIsDone.Checked)
            {
                frmTodoList newList = new frmTodoList(_todoList);
                _todoList.Remove(_id);
                Dispose();

            }
        }
    }
}