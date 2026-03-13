using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
// 確保引入正確的 Models 命名空間
using PoChunSu_BackendTest_MidLevel.Models; 

namespace PoChunSu_BackendTest_MidLevel.Controllers
{
    [Route("api/myofficeacpd")]
    [ApiController]
    public class MyofficeAcpdController : ControllerBase
    {
        private readonly MyofficeContext _context;

        public MyofficeAcpdController(MyofficeContext context)
        {
            _context = context;
        }

        // GET: api/myofficeacpd
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MyOfficeAcpd>>> GetAll()
        {
            try
            {
                // 注意這裡的 DbSet 名稱也配合改為大寫 O
                var data = await _context.MyOfficeAcpds.ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"伺服器內部錯誤: {ex.Message}");
            }
        }

        // GET: api/myofficeacpd/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MyOfficeAcpd>> GetById(string id)
        {
            var acpd = await _context.MyOfficeAcpds.FindAsync(id);

            if (acpd == null)
            {
                return NotFound();
            }

            return Ok(acpd);
        }

        // POST: api/myofficeacpd
        [HttpPost]
        public async Task<ActionResult<MyOfficeAcpd>> Create(MyOfficeAcpd myOfficeAcpd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 呼叫 SP 產生主鍵
// 新的專業寫法 (請貼上)
var connection = _context.Database.GetDbConnection();
await connection.OpenAsync();
using var command = connection.CreateCommand();

command.CommandText = "NEWSID";
command.CommandType = CommandType.StoredProcedure;

// 1. 設定傳入參數 @TableName
var tableParam = command.CreateParameter();
tableParam.ParameterName = "@TableName";
tableParam.Value = "Myoffice_ACPD";
command.Parameters.Add(tableParam);

// 2. 設定輸出參數 @ReturnSID
var returnParam = command.CreateParameter();
returnParam.ParameterName = "@ReturnSID";
returnParam.DbType = DbType.String;
returnParam.Size = 50; // 設定長度以接收字串
returnParam.Direction = ParameterDirection.Output;
command.Parameters.Add(returnParam);

// 3. 執行預存程序
await command.ExecuteNonQueryAsync();

// 4. 取得產生的 ID
var newId = returnParam.Value?.ToString() ?? Guid.NewGuid().ToString();

myOfficeAcpd.AcpdSid = newId;

                _context.MyOfficeAcpds.Add(myOfficeAcpd);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = myOfficeAcpd.AcpdSid }, myOfficeAcpd);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"新增失敗: {ex.Message}");
            }
        }

        // PUT: api/myofficeacpd/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, MyOfficeAcpd myOfficeAcpd)
        {
            if (id != myOfficeAcpd.AcpdSid)
            {
                return BadRequest("URL 的 ID 與資料本體的 ID 不符");
            }

            _context.Entry(myOfficeAcpd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MyOfficeAcpdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/myofficeacpd/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var myOfficeAcpd = await _context.MyOfficeAcpds.FindAsync(id);
            if (myOfficeAcpd == null)
            {
                return NotFound();
            }

            _context.MyOfficeAcpds.Remove(myOfficeAcpd);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MyOfficeAcpdExists(string id)
        {
            return _context.MyOfficeAcpds.Any(e => e.AcpdSid == id);
        }
    }
}