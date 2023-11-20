﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PontoCalculator.Data;
using PontoCalculator.Dtos;
using PontoCalculator.Helpers;
using PontoCalculator.Models;
using System.Net;

namespace PontoCalculator.Controllers
{
    [Route("api/")]
    [ApiController]
    public class PontoController : Controller
    {
        private readonly IPontoRepository _pontoRepository;
        private readonly JwtService _jwtService;

        public PontoController(IPontoRepository repository, JwtService jwtService)
        {
            _jwtService = jwtService;
            _pontoRepository = repository;
        }

        [HttpPost("ponto")]
        public IActionResult Register(string type, string? details)
        {
            int userId;
            try
            {
                var jwt = Request.Cookies["jwt"];
                var token = _jwtService.Verify(jwt);
                userId = int.Parse(token.Issuer);
            }
            catch (Exception ex) { return Unauthorized("Please login"); }

            var ponto = new Ponto
            {
                User_id = userId,
                DateTime = DateTime.Now,
                Type = type,
                Details = details
            };
            return Created("Success", _pontoRepository.Create(ponto));
        }

        [HttpGet("ponto")]
        public IActionResult Get(int? pontoId = null, bool? today = null)
        {
            int userId;
            try
            {
                var jwt = Request.Cookies["jwt"];
                var token = _jwtService.Verify(jwt);
                userId = int.Parse(token.Issuer);
            }
            catch (Exception ex){return Unauthorized("Please login");}

            List<Ponto> result = 
                _pontoRepository.Get(
                    userId: userId, 
                    pontoId: pontoId, 
                    today: today);

            if (result.IsNullOrEmpty()) {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
