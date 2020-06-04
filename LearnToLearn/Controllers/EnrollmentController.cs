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
    [RoutePrefix("api/enrollments")]
    public class EnrollmentController : ApiController
    {
        private BaseService<Enrollment> service;
        private UnitOfWork unitOfWork = new UnitOfWork();
        public EnrollmentController()
        {
            service = new BaseService<Enrollment>(new ModelStateWrapper(this.ModelState), unitOfWork);
        }

        public EnrollmentController(BaseService<Enrollment> service)
        {
            this.service = service;
        }

        [Route("")]
        [HttpGet]
        public List<Enrollment> GetAll()
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
            if (token.User.IsTeacher)
                {
                    return service.GetAll();
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("You don't have permission for this activity!");
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
            if (!token.User.IsTeacher)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("You don't have permission for this activity!");
                throw new HttpResponseException(message);
            }
            else
            {
                if (!EnrollmentExists(id))
                {
                    var enrollment = service.GetById(id);
                    if (enrollment == null)
                    {
                        return NotFound();
                    }
                    return Ok(enrollment);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("The enrollment doesn't exist!");
                    throw new HttpResponseException(message);
                }
            }        
        }

        [Route("{Id}")]
        [HttpPut]
        public IHttpActionResult PutEnrollment(int id, [FromBody] EnrollmentBindModel enrollmentBindmodel)
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

            if (!token.User.IsTeacher)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("You don't have permission for this activity!");
                throw new HttpResponseException(message);
            }
            else
            {
                if (!EnrollmentExists(id))
                {
                    Enrollment enrollment = new Enrollment();
                    enrollment.Id = id;
                    enrollment.UpdatedAt = DateTime.Now;
                    enrollment.CourseId = enrollmentBindmodel.CourseId;
                    enrollment.UserId = token.UserId;

                    try
                    {
                        service.Update(enrollment);
                        service.Save();
                    }
                    catch
                    {
                        if (!EnrollmentExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return Ok(enrollmentBindmodel);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("The enrollment doesn't exist!");
                    throw new HttpResponseException(message);
                }
            }
        }

        [Route("", Name = "Enrollment")]
        [HttpPost]
        public IHttpActionResult PostEnrollment([FromBody] Enrollment enrollment)
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
            if (token.User.IsTeacher)
                {
                    int userCount = 0;
                    BaseService<Enrollment> enrollmentService = new BaseService<Enrollment>(new ModelStateWrapper(this.ModelState), unitOfWork);
                    BaseService<Course> courseService = new BaseService<Course>(new ModelStateWrapper(this.ModelState), unitOfWork);
                    Course course = courseService.GetById(enrollment.CourseId);
                    List<Enrollment> enrollments = enrollmentService.GetAll(e => e.CourseId == enrollment.CourseId);
                    foreach (var item in enrollments)
                    {
                        userCount++;
                    }

                    if (course.Capacity > userCount)
                    {
                        service.Create(enrollment);
                        service.Save();

                        return CreatedAtRoute("Enrollment", new { id = enrollment.Id }, enrollment);
                    }
                    else
                    {
                        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                        message.Content = new StringContent("You can't enroll for this course, it is filled!");
                        throw new HttpResponseException(message);
                    }
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("You don't have permission for this activity!");
                    throw new HttpResponseException(message);
                }
        }

        //patch - grade
        [Route("{Id}")]
        [HttpPatch]
        public IHttpActionResult PatchEnrollment(int id, [FromBody] GradeModel gradeModel)
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

            if (!token.User.IsTeacher)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("You don't have permission for this activity!");
                throw new HttpResponseException(message);
            }
            else
            {
                if (!EnrollmentExists(id))
                {
                    var enrollment = service.GetById(id);
                    enrollment.Grade = gradeModel.Grade;
                    enrollment.UpdatedAt = DateTime.Now;
                    service.Update(enrollment);
                    service.Save();
                    return Ok(enrollment);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("The enrollment doesn't exist!");
                    throw new HttpResponseException(message);
                }
            }
        }

        [Route("{Id}")]
        [HttpDelete]
        public IHttpActionResult DeleteEnrollment(int id)
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

            if (!token.User.IsTeacher)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                message.Content = new StringContent("You don't have permission for this activity!");
                throw new HttpResponseException(message);
            }
            else
            {
                if (!EnrollmentExists(id))
                {
                    service.Delete(id);
                    service.Save();
                    return Ok();
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("The enrollment doesn't exist!");
                    throw new HttpResponseException(message);
                }
            }
        }

        private bool EnrollmentExists(int id)
        {
            bool flag = false;
            List<Enrollment> enrollments = service.GetAll();
            foreach (var enrollment in enrollments)
            {
                if (enrollment.Id == id)
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
