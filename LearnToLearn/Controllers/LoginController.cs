using LearnToLearn.BindModels;
using LearnToLearn.Models;
using LearnToLearn.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LearnToLearn.Controllers
{
    public class LoginController : ApiController
    {
        private BaseService<Token> service;
        private UnitOfWork unitOfWork = new UnitOfWork();

        public LoginController()
        {
            service = new BaseService<Token>(new ModelStateWrapper(this.ModelState), unitOfWork);
        }

        public LoginController(BaseService<Token> service)
        {
            this.service = service;
        }


        [Route("api/Login")]
        public IHttpActionResult Post(User user)
        {
            BaseService<User> bsu = new BaseService<User>(new ModelStateWrapper(this.ModelState), new UnitOfWork());
            List < User > users = bsu.GetAll();
            User loggedUser = bsu.GetAll(u => u.Email == user.Email && u.Password == user.Password).SingleOrDefault();

            BaseService<Token> serviceToken = new BaseService<Token>(new ModelStateWrapper(this.ModelState), new UnitOfWork());
            List<Token> tokens = serviceToken.GetAll();
            Token token = serviceToken.GetAll(t => t.UserId == loggedUser.Id).SingleOrDefault();

            if (tokens.Count == 0 || !tokens.Contains(serviceToken.GetAll(t => t.UserId == loggedUser.Id).SingleOrDefault()))
            {
                Token Newtoken = new Token()
                {
                    Name = GenerateToken.Generate(),
                    UserId = loggedUser.Id,
                    StartDate = DateTime.Now
                };
                Newtoken.EndDate = Newtoken.StartDate.AddMinutes(30);
                service.Create(Newtoken);
                service.Save();
                return Ok(Newtoken.Name);
            }

            if (token.EndDate >= DateTime.Now)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("This user already have a generated token - " + token.Name);
                throw new HttpResponseException(message);
            }
            else
            {
                serviceToken.Delete(token.Id);
                serviceToken.Save();
                Token newToken = new Token()
                {
                    Name = GenerateToken.Generate(),
                    UserId = loggedUser.Id,
                    StartDate = DateTime.Now
                };
                newToken.EndDate = newToken.StartDate.AddMinutes(30);
                service.Create(newToken);
                service.Save();
                return Ok(newToken.Name);
            }
        }

        [HttpPost]
        [Route("api/register")]
        public IHttpActionResult Post([FromBody] RegisterUserBindModel userBindModel)
        {
            User user = new User
            {
                Name = userBindModel.Name,
                Email = userBindModel.Email,
                Password = userBindModel.Password
            };
            UserService userService = new UserService(new ModelStateWrapper(this.ModelState), unitOfWork);
            userService.Create(user);
            userService.Save();

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }


    }
}
