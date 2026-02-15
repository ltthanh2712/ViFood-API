using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ViFoodAPI.Services.MongoDB;
using Oracle.ManagedDataAccess.Client;
namespace ViFoodAPI.Controllers.MongoDB;

[ApiController]
[Route("api/[controller]")]
public class OcrResultController : ControllerBase
{
    private readonly OcrResultService _service;

    public OcrResultController(OcrResultService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAllProductsAsync();

        var json = products.Select(p => p.ToJson());

        return Ok(json);
    }
    [HttpGet("{id:length(24)}")]
    public async Task<IActionResult> GetById(string id)
    {
        var product = await _service.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.ToJson());
    }
}
