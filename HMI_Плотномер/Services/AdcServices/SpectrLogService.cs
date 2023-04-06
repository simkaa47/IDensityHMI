using IDensity.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IDensity.Services.AdcServices
{
    public class SpectrLogService : PropertyChangedBase
    {
        string _path = "";
        string _parameters;
        IEnumerable<Point> _collection;
        public void WriteToFile(string path, string parameters, IEnumerable<Point> collection)
        {
            if (WriteTask == null || WriteTask.Status != TaskStatus.Running)
            {
                WriteTask = new Task(() => Write());
                _collection = collection;
                _path = path;
                _parameters = parameters;
                WriteTask.Start();
            }
        }

        Task WriteTask;
        void Write()
        {
            try
            {
                StringBuilder builder = new StringBuilder($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:f") + "\t"}" + _parameters + "\t");
                foreach (var point in _collection)
                {
                    builder.Append(point.Y + ";");
                }
                var text = builder.ToString();
                using (StreamWriter writer = new StreamWriter(_path, true))
                {
                    writer.WriteLine(text);
                }

            }
            catch (Exception ex)
            {

                SpectrErrorEvent?.Invoke(ex.Message);
            }
            finally
            {

            }
        }

        public event Action<string> SpectrErrorEvent;
    }
}
