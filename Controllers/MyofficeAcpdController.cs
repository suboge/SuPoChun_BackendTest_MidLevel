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
returnParam.Size = 50; 
returnParam.Direction = ParameterDirection.Output;
command.Parameters.Add(returnParam);

// 3. 執行預存程序
await command.ExecuteNonQueryAsync();

// 4. 取得產生的 ID
var newId = returnParam.Value?.ToString() ?? Guid.NewGuid().ToString();

myOfficeAcpd.AcpdSid = newId;

                _context.MyOfficeAcpds.Add(myOfficeAcpd);
                await LogExecution("CreateAcpd", myOfficeAcpd);
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
        private async Task LogExecution(string action, object data)
{
    try
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "usp_AddLog";
        command.CommandType = CommandType.StoredProcedure;

        // 設定 SP 要求的參數
        var p1 = command.CreateParameter(); p1.ParameterName = "@_InBox_ReadID"; p1.Value = 0; command.Parameters.Add(p1);
        var p2 = command.CreateParameter(); p2.ParameterName = "@_InBox_SPNAME"; p2.Value = "MyofficeAcpdController"; command.Parameters.Add(p2);
        var p3 = command.CreateParameter(); p3.ParameterName = "@_InBox_GroupID"; p3.Value = Guid.NewGuid(); command.Parameters.Add(p3);
        var p4 = command.CreateParameter(); p4.ParameterName = "@_InBox_ExProgram"; p4.Value = action; command.Parameters.Add(p4);
        var p5 = command.CreateParameter(); p5.ParameterName = "@_InBox_ActionJSON"; p5.Value = System.Text.Json.JsonSerializer.Serialize(data); command.Parameters.Add(p5);

        // Output 參數 (雖然我們不一定會用到回傳值，但 SP 要求就必須給)
        var pOut = command.CreateParameter();
        pOut.ParameterName = "@_OutBox_ReturnValues";
        pOut.Size = -1; // nvarchar(max)
        pOut.Direction = ParameterDirection.Output;
        command.Parameters.Add(pOut);

        await command.ExecuteNonQueryAsync();
    }
    catch
    {
        // Log 失敗不應中斷主流程
    }
}
    }
}

