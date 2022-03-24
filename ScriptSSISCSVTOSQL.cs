#region Help:  Introduction to the script task
/* The Script Task allows you to perform virtually any operation that can be accomplished in
 * a .Net application within the context of an Integration Services control flow. 
 * 
 * Expand the other regions which have "Help" prefixes for examples of specific ways to use
 * Integration Services features within this script task. */
#endregion


#region Namespaces
using System;
using System.Data;
using Microsoft.SqlServer.Dts.Runtime;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
#endregion

namespace ST_d805385425254105a21434feddb58a6b
{
   
	[Microsoft.SqlServer.Dts.Tasks.ScriptTask.SSISScriptTaskEntryPointAttribute]
	public partial class ScriptMain : Microsoft.SqlServer.Dts.Tasks.ScriptTask.VSTARTScriptObjectModelBase
	{
        
		public void Main()
		{            

            string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {

                //PEGANDO O VALOR DOS PARAMETROS
                string SourceFolderPath = Dts.Variables["User::SourceFolder"].Value.ToString();
                string FileExtension = Dts.Variables["User::FileExtension"].Value.ToString();
                string FileDelimiter = Dts.Variables["User::FileDelimiter"].Value.ToString();
                string ArchiveFolder = Dts.Variables["User::ArchiveFolder"].Value.ToString();
                string ColumnsDataType = Dts.Variables["User::ColumnsDataType"].Value.ToString();
                string SchemaName = Dts.Variables["User::Schema"].Value.ToString();
                string QtdLinhas = Dts.Variables["User::QtdRows"].Value.ToString();                


                //Leitura dos arquivos na pasta
                string[] fileEntries = Directory.GetFiles(SourceFolderPath, "*" + FileExtension);
                foreach (string fileName in fileEntries)
                {

                    SqlConnection myADONETConnection = new SqlConnection();
                    myADONETConnection = (SqlConnection)
           (Dts.Connections["DBEnem"].AcquireConnection(Dts.Transaction) as SqlConnection);

                    //Inserindo os dados na tabela
                    string TableName = "";
                    int counter = 0;
                    string line;
                    string ColumnList = "";
                    // VERIFICAR SE TENHO LIMITAÇÃO DE LINHAS                            
                    bool temLimite = QtdLinhas != null && QtdLinhas.Trim().Length >= 1;
                    
                    System.IO.StreamReader SourceFile =
                    new System.IO.StreamReader(fileName);
                    while ((line = SourceFile.ReadLine()) != null)
                    {
                        if (counter == 0)
                        {
                            ColumnList = "[" + line.Replace(FileDelimiter, "],[") + "]";
                            TableName = (((fileName.Replace(SourceFolderPath, "")).Replace(FileExtension, "")).Replace("\\", ""));
                            string CreateTableStatement = "IF EXISTS (SELECT * FROM sys.objects WHERE name = '" + TableName+"'";
                            
                            CreateTableStatement += " AND type in (N'U'))DROP TABLE [" + SchemaName + "].[dbo].";
                            CreateTableStatement += "[" + TableName + "]  Create Table " + SchemaName + ".[dbo].[" + TableName + "]";
                            CreateTableStatement += "([" + line.Replace(FileDelimiter, "] " + ColumnsDataType + ",[") + "] " + ColumnsDataType + ")";
                            SqlCommand CreateTableCmd = new SqlCommand(CreateTableStatement, myADONETConnection);
                            CreateTableCmd.ExecuteNonQuery();                            

                        }
                        else
                        {
                            
                            if (temLimite && counter > int.Parse(QtdLinhas))
                            {
                                return;
                            }
                            else
                            {
                                string query = "Insert into " + SchemaName + ".[dbo].[" + TableName + "] (" + ColumnList + ") ";
                                query += "VALUES('" + line.Replace(FileDelimiter, "','") + "')";
                                
                                SqlCommand myCommand1 = new SqlCommand(query, myADONETConnection);
                                myCommand1.ExecuteNonQuery();
                            }
                        }

                        counter++;
                    }

                    SourceFile.Close();                    
                    File.Move(fileName, ArchiveFolder + "\\" + (fileName.Replace(SourceFolderPath, "")).Replace(FileExtension, "") + "_" + datetime + FileExtension);
                    Dts.TaskResult = (int)ScriptResults.Success;
                }
            }
            catch (Exception exception)
            {
                // Criando o log caso algo dê errado
                using (StreamWriter sw = File.CreateText(Dts.Variables["User::LogFolder"].Value.ToString()
                                    + "\\" + "ErrorLog_" + datetime + ".log"))
                {
                    sw.WriteLine(exception.ToString());
                    Dts.TaskResult = (int)ScriptResults.Failure;
                }

            }
        }

        #region ScriptResults declaration
       
        enum ScriptResults
        {
            Success = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success,
            Failure = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure
        };
        #endregion

	}
}