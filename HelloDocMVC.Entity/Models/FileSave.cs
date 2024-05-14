using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class FileSave
    {
        #region UploadFile
        public static string UploadDoc(IFormFile UploadFile, int Requestid)
        {
            string upload_path = null;
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload\\" + Requestid;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";
                string fileNameWithPath = Path.Combine(path, newfilename);
                upload_path = FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + newfilename;
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            return upload_path;
        }
        #endregion
        #region UploadFile
        public static string UploadProviderDoc(IFormFile UploadFile, int Physicianid, string FileName)
        {
            string upload_path = null;
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload\\Physician\\" + Physicianid;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string newfilename = FileName;

                string fileNameWithPath = Path.Combine(path, newfilename);
                upload_path = FilePath.Replace("wwwroot\\Upload\\Physician\\", "/Upload/Physician/") + "/" + newfilename;


                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }


            }
            return upload_path;
        }
        #endregion
        #region UploadFile_TimeSheet
        public static string UploadTimesheetDoc(IFormFile UploadFile, int TimeSheetId)
        {
            string newfilename = null;
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload\\TimeSheet\\" + TimeSheetId;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}"; ;

                string fileNameWithPath = Path.Combine(path, newfilename);
                //upload_path = FilePath.Replace("wwwroot\\Upload\\TimeSheet\\", "/Upload/TimeSheet/") + "/" + newfilename;


                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            return newfilename;
        }
        #endregion
    }
}
