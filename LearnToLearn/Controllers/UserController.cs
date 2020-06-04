using LearnToLearn.BindModels;
using LearnToLearn.DataAccess;
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
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private BaseService<User> service;

        public UserController()
        {
            service = new BaseService<User>(new ModelStateWrapper(this.ModelState), unitOfWork);
        }

        public UserController(BaseService<User> service)
        {
            this.service = service;
        }

        [Route("")]
        [HttpGet]
        public List<UserBindModel> GetAll()
        {
            var re = Request;
            var header = re.Headers;
            string h;
            try
            {
                h = header.GetValues("Authorization").First();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            TokenService bsu = new TokenService(new ModelStateWrapper(this.ModelState), unitOfWork);
            Token token = bsu.GetByName(h);
            User loggedUser = service.GetById(token.UserId);
                if (loggedUser.IsTeacher)
                {
                    List<User> users = service.GetAll();
                    List<UserBindModel> usersBindModel = new List<UserBindModel>();
                    foreach (var user in users)
                    {
                        UserBindModel userBindModel = new UserBindModel
                        {
                            Name = user.Name,
                            Email = user.Email,
                            Password = user.Password,
                            IsTeacher = user.IsTeacher
                        };
                        usersBindModel.Add(userBindModel);
                    }
                    return usersBindModel;
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("You don't have permission for this action!");
                    throw new HttpResponseException(message);
                }
        }

        [HttpGet]
        [Route("{Id}")]
        public IHttpActionResult GetById(int id)
        {
            var re = Request;
            var header = re.Headers;
            string h;
            try
            {
                h = header.GetValues("Authorization").First();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            TokenService bsu = new TokenService(new ModelStateWrapper(this.ModelState), unitOfWork);
            Token token = bsu.GetByName(h);
            User loggedUser = service.GetById(token.UserId);

            if (!loggedUser.IsTeacher && loggedUser.Id != id)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("You don't have permission for this action!");
                throw new HttpResponseException(message);
            }
            else
            {
                if (loggedUser.Id == id || loggedUser.IsTeacher)
                {
                    var user = service.GetById(id);
                    UserBindModel userBindModel = new UserBindModel
                    {
                        Name = user.Name,
                        Email = user.Email,
                        Password = user.Password,
                        IsTeacher = user.IsTeacher
                    };
                    if (user == null)
                    {
                        return NotFound();
                    }
                    return Ok(userBindModel);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("You don't have permission for this action!");
                    throw new HttpResponseException(message);
                }
            }
        }

        [Route("{Id}")]
        [HttpPut]
        public IHttpActionResult PutUser(int id, [FromBody] UserBindModel userBindModel)
        {
            var re = Request;
            var header = re.Headers;
            string h;
            try
            {
                h = header.GetValues("Authorization").First();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            TokenService bsu = new TokenService(new ModelStateWrapper(this.ModelState), unitOfWork);
            Token token = bsu.GetByName(h);
            User loggedUser = service.GetById(token.UserId);


            if (!loggedUser.IsTeacher)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("You don't have permission for this action!");
                throw new HttpResponseException(message);
            }
            else
            {
                if (!UserExists(id))
                {
                    User user = new User
                    {
                        Name = userBindModel.Name,
                        Email = userBindModel.Email,
                        Password = userBindModel.Password,
                        IsTeacher = userBindModel.IsTeacher
                    };
                    if (id != user.Id)
                    {
                        return BadRequest();
                    }

                    try
                    {
                        service.Update(user);
                        service.Save();
                    }
                    catch
                    {
                        if (!UserExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("The user doesn't exist!");
                    throw new HttpResponseException(message);
                }
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult PostUser([FromBody] UserBindModel userBindModel)
        {
            var re = Request;
            var header = re.Headers;
            string h;
            try
            {
                h = header.GetValues("Authorization").First();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            TokenService bsu = new TokenService(new ModelStateWrapper(this.ModelState), unitOfWork);
            Token token = bsu.GetByName(h);
            User loggedUser = service.GetById(token.UserId);
                if (loggedUser.IsTeacher)
                {
                    User user = new User
                    {
                        Name = userBindModel.Name,
                        Email = userBindModel.Email,
                        Password = userBindModel.Password,
                        IsTeacher = userBindModel.IsTeacher
                    };
                    service.Create(user);
                    service.Save();

                    return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("You don't have permission for this action!");
                    throw new HttpResponseException(message);
                }
        }

        [Route("{Id}")]
        [HttpDelete]
        public IHttpActionResult DeleteUser(int id)
        {
            var re = Request;
            var header = re.Headers;
            string h;
            try
            {
                h = header.GetValues("Authorization").First();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            TokenService bsu = new TokenService(new ModelStateWrapper(this.ModelState), unitOfWork);
            Token token = bsu.GetByName(h);
            User loggedUser = service.GetById(token.UserId);

            if (!loggedUser.IsTeacher)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("You don't have permission for this action!");
                throw new HttpResponseException(message);
            }
            else
            {
                
                if (!UserExists(id))
                {
                    User user = service.GetById(id);
                    user.Courses.Clear();
                    service.Delete(id);
                    service.Save();
                    return Ok();
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("The user doesn't exist!");
                    throw new HttpResponseException(message);
                }
            }
        }

        [Route("{Id}/Enrollments")]
        [HttpGet]
        public List<AllEnrollmentsBindModel> GetAllEnrollments(int id)
        {
            var re = Request;
            var header = re.Headers;
            string h;
            try
            {
                h = header.GetValues("Authorization").First();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            TokenService bsu = new TokenService(new ModelStateWrapper(this.ModelState), unitOfWork);
            Token token = bsu.GetByName(h);
            User user = service.GetById(token.UserId);
            if (user.Id !=id)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("You don't have permission for this action!");
                throw new HttpResponseException(message);
            }
            else
            {
                List<Enrollment> enrollments = user.Enrollments;
                List<AllEnrollmentsBindModel> bindModelEnrollments = new List<AllEnrollmentsBindModel>();
                foreach (var enrollment in enrollments)
                {
                    AllEnrollmentsBindModel bindModel = new AllEnrollmentsBindModel
                    {
                        CourseName = enrollment.Course.Name,
                        Grade = enrollment.Grade,
                        CreatedAt = enrollment.CreatedAt
                    };
                    bindModelEnrollments.Add(bindModel);
                }

                return bindModelEnrollments;
            }
        }

        [Route("{Id}/Enrollments")]
        [HttpPost]
        public IHttpActionResult PostEnrollment([FromBody] EnrollmentBindModel  enrollmentBindModel)
        {
            var re = Request;
            var header = re.Headers;
            string h;
            try
            {
                h = header.GetValues("Authorization").First();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            TokenService bsu = new TokenService(new ModelStateWrapper(this.ModelState), unitOfWork);
            Token token = bsu.GetByName(h);

            int userCount = 0;
            BaseService<Enrollment> enrollmentService = new BaseService<Enrollment>(new ModelStateWrapper(this.ModelState), unitOfWork);
            BaseService<Course> courseService = new BaseService<Course>(new ModelStateWrapper(this.ModelState), unitOfWork);
            Course course = courseService.GetById(enrollmentBindModel.CourseId);
            List<Enrollment> enrollments = enrollmentService.GetAll(e => e.CourseId == enrollmentBindModel.CourseId);
            List<Enrollment> userEnrollments = enrollmentService.GetAll(e => e.UserId == token.UserId);

            foreach (var item in enrollments)
            {
                userCount++;
            }

            bool flag = true;
            foreach (var item in userEnrollments)
            {
                if (item.UserId == token.UserId && item.CourseId == enrollmentBindModel.CourseId)
                {
                    flag = false;
                }
            }

            if (flag == false)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("You have already enrolled for this course!");
                throw new HttpResponseException(message);
            }
            else
            {
                if (course.Capacity > userCount)
                {
                    Enrollment enrollment = new Enrollment
                    {
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        UserId = token.UserId,
                        CourseId = enrollmentBindModel.CourseId
                    };

                    PostEnrollmentBindModel allEnrollmentsBindModel = new PostEnrollmentBindModel
                    {
                        CourseId = enrollmentBindModel.CourseId,
                        Grade = enrollment.Grade,
                        CreatedAt = enrollment.CreatedAt
                    };


                    enrollmentService.Create(enrollment);
                    enrollmentService.Save();

                    return Ok(allEnrollmentsBindModel);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("You can't enroll for this course, it is filled!");
                    throw new HttpResponseException(message);
                }
            }    
        }

        private bool UserExists(int id)
        {
            bool flag = false;
            List<User> users = service.GetAll();
            foreach (var user in users)
            {
                if (user.Id == id)
                {
                    flag = true;
                    break;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }
    }
}
