using CsvHelper;
using CsvHelper.Configuration;
using SyncManager.Helper.CSV;
using SyncManager.Helper.Extension;
using SyncManager.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SyncManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            TextReader fileReadableStream = null;
            string connectionString = GetConnectionString();

            Console.WriteLine("Enter FileName with extension and path");
            string location = Console.ReadLine();

            Console.WriteLine("Enter Reporting Time");
            var reportingTime = Console.ReadLine();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            CsvConfiguration config = new CsvConfiguration();
            config.Delimiter = ",";
            config.RegisterClassMap<CustomClassMapForEnrollment>();


            if (location != null)
            {
                var fileStream = File.ReadAllText(location);

                fileReadableStream = new StringReader(fileStream);

                var csv = new CsvReader(fileReadableStream, config);

                var records = csv.GetRecords<HealthEnrollment>().ToList();
                List<HealthEnrollmentDTO> healthEnrollmentDto = new List<HealthEnrollmentDTO>();

                foreach (var healthEnrollment in records)
                {
                    HealthEnrollmentDTO enrollment = new HealthEnrollmentDTO()
                    {
                        ContractNumber = healthEnrollment.ContractNumber,
                        State = healthEnrollment.State,
                        FIPSStateCountyCode = healthEnrollment.FIPSStateCountyCode,
                        SSAStateCountyCode = healthEnrollment.SSAStateCountyCode,
                        County = healthEnrollment.County,
                        PlanID = healthEnrollment.PlanID,
                        Enrollment = healthEnrollment.Enrollment,
                        ReportingTime = reportingTime
                    };
                    healthEnrollmentDto.Add(enrollment);
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.BatchSize = 100;
                        bulkCopy.DestinationTableName = "dbo.Enrollment";
                        try
                        {
                            bulkCopy.WriteToServer(healthEnrollmentDto.AsDataTable());
                        }
                        catch (Exception)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            watch.Stop();
            var elapsedTime = watch.Elapsed;
            Console.WriteLine("Elapsed Time : H:{0} M:{1} S:{2}", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
            Console.ReadLine();
        }

        private static string GetConnectionString()
        {
            //TODO: later on pick it from App.Config
            return "Data Source=.;Initial Catalog=Enrolment;Persist Security Info=True; User ID=admin;Password=12345;";
        }
    }
}
