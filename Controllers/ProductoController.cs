using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.Models;
using ProyectoGraduación.IServices;
    

[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
    private readonly IProductoService _service;

    public ProductoController(IProductoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await _service.GetById(id));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Producto producto)
    {
        await _service.Add(producto);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Producto producto)
    {
        producto.Id = id;
        await _service.Update(producto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return Ok();
    }
}
