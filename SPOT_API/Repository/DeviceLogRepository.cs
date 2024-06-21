
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
            connectionString = configuration.GetValue<string>("ConnectionStrings:IotDB");
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
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

    //            dbConnection.Execute("INSERT INTO public.devicedata(" +
    //"deviceid, createdate, data, latitude, longitude, alarm, batt, mode, fw_ver, hw_ver, wifi_rssi, ble_rssi, date_time, prox_id_list, is_processed)" +
    //"VALUES(@deviceid, @createdate, @data, @latitude, @longitude, @alarm, @batt, @mode, @fw_ver, @hw_ver, @wifi_rssi, @ble_rssi, @date_time, @prox_id_list, @is_processed)", item);


                var guid = dbConnection.ExecuteScalar<Guid>($"INSERT INTO public.devicedata(" +
    "deviceid, createdate, data, latitude, longitude, alarm, batt, mode, fw_ver, hw_ver, wifi_rssi, ble_rssi, date_time, prox_id_list, is_processed)" +
    "VALUES(@deviceid, @createdate, @data, @latitude, @longitude, @alarm, @batt, @mode, @fw_ver, @hw_ver, @wifi_rssi, @ble_rssi, @date_time, @prox_id_list, @is_processed) RETURNING Id", item);

                item.Id = guid;
                //return item;
            }

        }

        public  IEnumerable<DeviceLog> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<DeviceLog>("SELECT * FROM devicedata order by createdate desc");
            }
        }

        public  DeviceLog FindByID(Guid id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return  dbConnection.Query<DeviceLog>("SELECT * FROM devicedata WHERE id = @Id", new { Id = id }).FirstOrDefault();
            }
        }

        public void Remove(Guid id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM devicedata WHERE Id=@Id", new { Id = id });
            }
        }

        public void Update(DeviceLog item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Query("UPDATE devicedata SET is_processed = @is_processed WHERE id = @Id", item);
            }
        }
    }
}
