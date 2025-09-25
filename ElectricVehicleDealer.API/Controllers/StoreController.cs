﻿using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElectricVehicleDealer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _service;

        public StoreController(IStoreService service)
        {
            _service = service;
        }
    

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _service.GetAllStoresAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var store = await _service.GetStoreByIdAsync(id);
            return Ok(store);
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateStoreRequest dto)
        {
            var created = await _service.CreateStoreAsync(dto);
            return Ok(created);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateStoreRequest dto)
        {
            var updated = await _service.UpdateStoreAsync(id, dto);
            return Ok(updated);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteStoreAsync(id);
            if (!deleted) return NotFound();
            return Ok("Deleted successfully");
        }
    } 
}