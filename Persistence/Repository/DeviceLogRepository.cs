
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using Npgsql;
using SPOT_API.Models;
using System.Threading.Tasks;
using System;
//using ASPCoreSample.Models;

namespace SPOT_API.Repository
{
    public class DeviceLogRepository : IRepository<DeviceLog>
    {
        private string connectionString;
        public DeviceLogRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("IotDB");
        }

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }

        public void Add(DeviceLog item)
        {
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();

                    //            dbConnection.Execute("INSERT INTO public.devicedata(" +
                    //"deviceid, createdate, data, latitude, longitude, alarm, batt, mode, fw_ver, hw_ver, wifi_rssi, ble_rssi, date_time, prox_id_list, is_processed)" +
                    //"VALUES(@deviceid, @createdate, @data, @latitude, @longitude, @alarm, @batt, @mode, @fw_ver, @hw_ver, @wifi_rssi, @ble_rssi, @date_time, @prox_id_list, @is_processed)", item);


                    var guid = dbConnection.ExecuteScalar<int>($"INSERT INTO public.devicedata(" +
        "deviceid, createdate, data, latitude, longitude, alarm, batt, mode, fw_ver, hw_ver, wifi_rssi, ble_rssi, date_time, prox_id_list, dashboard_status)" +
        "VALUES(@deviceid, @createdate, @data, @latitude, @longitude, @alarm, @batt, @mode, @fw_ver, @hw_ver, @wifi_rssi, @ble_rssi, @date_time, @prox_id_list, @dashboard_status) RETURNING Id", item);

                    item.Id = guid;
                    //return item;
                }
            }
            catch (Exception ex)
            {
            }

        }

        public IEnumerable<DeviceLog> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<DeviceLog>("SELECT * FROM devicedata where createdate >= '2021-08-01'::date order by date_time desc LIMIT 10000");
                //where createdate >= '2021-09-08'::date order by date_time desc
            }
        }

        public DeviceLog FindByID(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<DeviceLog>("SELECT * FROM devicedata WHERE id = @Id", new { Id = id }).FirstOrDefault();
            }
        }

        public void Remove(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM devicedata WHERE Id=@Id", new { Id = id });
            }
        }

        public IEnumerable<DeviceLog> FindAllBy(string deviceId, DateTime start, DateTime end)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<DeviceLog>("SELECT * FROM devicedata where deviceid=@deviceId " +
                    "and createdate BETWEEN @fromDate::timestamp AND @toDate::timestamp", new
                    {
                        deviceId = deviceId,
                        fromDate = start.ToString("yyyy-MM-dd HH:mm:ss"),
                        toDate = end.ToString("yyyy-MM-dd HH:mm:ss")
                    });
            }

            //using (IDbConnection dbConnection = Connection)
            //{
            //    dbConnection.Open();
            //    return dbConnection.Query<DeviceLog>("SELECT * FROM devicedata WHERE id = @Id", new { Id = id }).FirstOrDefault();
            //}
        }

        public void Update(DeviceLog item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Query("UPDATE devicedata SET dashboard_status = @dashboard_status WHERE id = @Id", item);
            }
        }

        public IEnumerable<string> FindUniqueDevice()
        {
            List<string> devices = new List<string>();
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //var deviceLogs =  dbConnection.Query<DeviceLog>("SELECT DISTINCT deviceid FROM devicedata where dashboard_status IS NULL");
                var deviceLogs = dbConnection.Query<DeviceLog>("SELECT DISTINCT deviceid FROM devicedata where dashboard_status IS NULL OR dashboard_status = '' ");
                foreach (var d in deviceLogs)
                    devices.Add(d.deviceid);
            }
            return devices;

        }

        public IEnumerable<DeviceLog> FindAllPendingByDeviceName(string deviceName)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //return dbConnection.Query<DeviceLog>("SELECT * FROM devicedata where deviceid=@deviceId  AND (dashboard_status IS NULL OR dashboard_status = '') order by createdate desc LIMIT 10000", new
                return dbConnection.Query<DeviceLog>("SELECT * FROM devicedata where createdate >= '2021-08-01'::date AND deviceid=@deviceId  AND (dashboard_status IS NULL OR dashboard_status = '') order by date_time desc LIMIT 10000", new
                {
                    deviceId = deviceName,
                    });
            }
        }
    }
}
