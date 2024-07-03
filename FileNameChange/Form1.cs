using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileNameChange
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void btnChange_Click(object sender, EventArgs e)
        {
            //IniFile ini = new IniFile();

            //ini.Load("D:\\Project\\FileNameChange\\FileNameChange\\Config.ini");
            //string TextResult = ini["Setting"]["TextResult"].ToString();
            string TextResult = "";

            TextResult = System.Configuration.ConfigurationSettings.AppSettings["keyWord"];

            string result = TextResult.Replace(" ", "");
            string[] jsonR = result.Split(',');
            
            int cnt = 0;
            for (int i = 0; i < jsonR.Length; i++)
            {
                string strVal = jsonR[i];
                string[] name = strVal.Split(':');
                string oldName = name[0];  //기존값
                string newName = name[1]; //변경값

                DataTable dt_filelistinfo = GetFileListFromFolderPath(lblFolder.Text);

                
                for(int d = 0; d < dt_filelistinfo.Rows.Count; d++)
                {
                    string folderName = dt_filelistinfo.Rows[d]["Folder"].ToString();

                    string oldFileName = dt_filelistinfo.Rows[d]["FileName"].ToString();
                    string newFileName = oldFileName.Replace(oldName, newName);
                    
                    string fullFileNmae = folderName + "\\" + oldFileName;
                    string fullFileNameChange = folderName + "\\" + newFileName;

                    if(oldFileName != newFileName)
                    {
                        FileInfo fileN = new FileInfo(fullFileNameChange);

                        if (fileN.Exists)
                        {
                            MessageBox.Show("[" + newFileName + "] 파일이 존재합니다.");
                            return;
                        }
                        else
                        {
                            System.IO.File.Move(fullFileNmae, fullFileNameChange);
                            cnt += 1;
                        }
                    }
                }
            }
            MessageBox.Show(cnt + "건이 처리 완료되었습니다.");

            DataTable dt = GetFileListFromFolderPath(lblFolder.Text);
            ShowDataFromDataTableToDataGridView(dt, dataGridView1);
        }

        private DataTable GetFileListFromFolderPath(string FolderName)
        {
            DirectoryInfo di = new DirectoryInfo(FolderName); // 해당 폴더 정보를 가져옵니다.

            DataTable dt1 = new DataTable(); // 새로운 테이블 작성합니다.(FileInfo 에서 가져오기 원하는 속성을 열로 추가합니다.)
            dt1.Columns.Add("Folder", typeof(string)); // 파일의 폴더
            dt1.Columns.Add("FileName", typeof(string)); // 파일 이름(확장자 포함)
            //dt1.Columns.Add("Extension", typeof(string)); // 확장자
            dt1.Columns.Add("CreationTime", typeof(DateTime)); // 생성 일자
            dt1.Columns.Add("LastWriteTime", typeof(DateTime)); // 마지막 수정 일자
            dt1.Columns.Add("LastAccessTime", typeof(DateTime)); // 마지막 접근 일자

            foreach (FileInfo File in di.GetFiles()) // 선택 폴더의 파일 목록을 스캔합니다.
            {
                dt1.Rows.Add(File.DirectoryName, File.Name, File.CreationTime, File.LastWriteTime, File.LastAccessTime); // 개별 파일 별로 정보를 추가합니다.
            }

            if (chkFolder.Checked == true) // 하위 폴더 포함될 경우
            {
                DirectoryInfo[] di_sub = di.GetDirectories(); // 하위 폴더 목록들의 정보 가져옵니다.
                foreach (DirectoryInfo di1 in di_sub) // 하위 폴더목록을 스캔합니다.
                {
                    foreach (FileInfo File in di1.GetFiles()) // 선택 폴더의 파일 목록을 스캔합니다.
                    {
                        dt1.Rows.Add(File.DirectoryName, File.Name, File.CreationTime, File.LastWriteTime, File.LastAccessTime); // 개별 파일 별로 정보를 추가합니다.
                    }
                }
            }

            return dt1;
        }

        private void ShowDataFromDataTableToDataGridView(DataTable dt1, DataGridView dgv1)
        {
            dgv1.Rows.Clear(); // 이전 정보가 있을 경우, 모든 행을 삭제합니다.
            dgv1.Columns.Clear(); // 이전 정보가 있을 경우, 모든 열을 삭제합니다. 

            foreach (DataColumn dc1 in dt1.Columns) // 선택한 파일 목록이 들어있는 DataTable의 모든 열을 스캔합니다. 
            {
                dgv1.Columns.Add(dc1.ColumnName, dc1.ColumnName); // 출력할 DataGridView에 열을 추가합니다. 
            }

            int row_index = 0; // 행 인덱스 번호(초기 값) 
            foreach (DataRow dr1 in dt1.Rows) // 선택한 파일 목록이 들어있는 DataTable의 모든 행을 스캔합니다. 
            {
                dgv1.Rows.Add(); // 빈 행을 하나 추가합니다. 
                foreach (DataColumn dc1 in dt1.Columns) // 선택한 파일 목록이 들어있는 DataTable의 모든 열을 스캔합니다. 
                {
                    dgv1.Rows[row_index].Cells[dc1.ColumnName].Value = dr1[dc1.ColumnName]; // 선택 행 별로, 스캔하는 열에 해당하는 셀 값을 입력합니다. 
                }
                row_index++; // 다음 행 인덱스를 선택하기 위해 1을 더해줍니다. 
            }

            foreach (DataGridViewColumn drvc1 in dgv1.Columns) // 결과를 출력할 DataGridView의 모든 열을 스캔합니다. 
            {
                drvc1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; // 선택 열의 너비를 자동으로 설정합니다. 
            }
        }
            

            private void button2_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog(); // 새로운 폴더 선택 Dialog 를 생성합니다. 

            dialog.IsFolderPicker = true; // 
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) // 폴더 선택이 정상적으로 되면 아래 코드를 실행합니다. 
            {
                this.lblFolder.Text = dialog.FileName; // 선택한 폴더 이름을 label2에 출력합니다. 
                DataTable dt_filelistinfo = GetFileListFromFolderPath(dialog.FileName);
                ShowDataFromDataTableToDataGridView(dt_filelistinfo, dataGridView1);
            }
            
        }
    }
}
