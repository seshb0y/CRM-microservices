using ClientService.DTO.Responses;
using ClientService.Data.Models;
using ClientService.DTO.Requests.Client;
using ClientService.Services.Interfaces;
using CRMSolution.DTO.Requests.Client;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientService.Controllers;

[ApiController]
[Route("api/v1/clients/")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }


    [HttpPost]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> AddClient([FromBody] HttpCreateClientRequest request)
    {
        
        return Ok("await _clientService.CreateClient(request)");
    }
    
    [HttpPut]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> ChangeClient([FromBody] HttpChangeDataClientRequest request, 
        [FromServices] IValidator<HttpChangeDataClientRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        
        return Ok("await _clientService.ChangeDataClient(request)");
    }
    
    [HttpDelete]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> DeleteClient([FromBody] HttpDeleteClientRequest request)
    {
        // await _clientService.DeleteClient(request);
        return Ok("Client deleted");
    }
    
    [HttpGet("search")]
    // [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> LoadClientData([FromQuery] HttpFindClientRequest request)
    {
        return Ok("await _clientService.FindClient(request)");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClients([FromQuery] HttpSortClientsRequest httpSortClientsRequest)
    {
        // var clients = await _clientService.GetAllClients(httpSortClientsRequest);
        return Ok("clients");
    }
    
    [HttpGet("relations")]
    public async Task<IActionResult> GetClientsWithOrdersAndTasks()
    {
        // var clients = await _clientService.GetClientsWithOrdersAndTasks(HttpContext);
        return Ok();
    }



}