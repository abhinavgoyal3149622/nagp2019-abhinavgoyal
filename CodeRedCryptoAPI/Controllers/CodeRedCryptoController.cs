using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CodeRedCryptoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeRedCryptoController : ControllerBase
    {
        private ICodeRedCrypto _codeRedCrypto;

        ILogger _logger;

        public CodeRedCryptoController(ICodeRedCrypto codeRedCrypto, ILogger logger)
        {
            _codeRedCrypto = codeRedCrypto;
            _logger = logger;
        }
        /// <summary>
        /// Hash and Encrypt Password
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] PasswordRequestModel passwordData)
        {
            try
            {
                if (passwordData == null || String.IsNullOrEmpty(passwordData.PlainText))
                {
                    _logger.Error("Either Password Data is null or Plain text is null");
                    return BadRequest(" Either Password Data is null or Plain text is null");
                }
                var passwordResponse = _codeRedCrypto.Encrypt(passwordData);
                return Ok(passwordResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"Argon2 error :- {ex}");
                return BadRequest($"Argon2 error :- {ex}");
            }
        }
    }
}
