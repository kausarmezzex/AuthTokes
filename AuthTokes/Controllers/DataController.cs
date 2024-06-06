using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AuthTokes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private static readonly List<DataModel> DataList = new List<DataModel>();
        private readonly IConfiguration _configuration;

        public DataController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("add")]
        [Authorize]
        public IActionResult AddData([FromBody] DataModel model)
        {
            // Add model to DataList
            DataList.Add(model);

            return Ok(new { Message = "Data added successfully" });
        }

        [HttpGet("all")]
        [Authorize]
        public IActionResult GetAllData()
        {
            // Initialize the DataTable
            DataTable dataTable = new DataTable();

            // Use the connection string from IConfiguration
            string connectionString = _configuration.GetConnectionString("BloggieDConnectionString");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Open the database connection
                con.Open();

                // Query to fetch data from the BlogPosts table
                string query = "SELECT * FROM BlogPosts";

                // Execute the query
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Read the data into the DataTable
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }
            }

            // Convert DataTable to a list of dictionaries
            var dataList = new List<Dictionary<string, object>>();
            foreach (DataRow row in dataTable.Rows)
            {
                var dataObject = new Dictionary<string, object>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    dataObject[col.ColumnName] = row[col];
                }
                dataList.Add(dataObject);
            }

            // Return the data as JSON
            return Ok(dataList);
        }


    }

    public class DataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
