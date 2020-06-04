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
    [RoutePrefix("api/courses")]
    public class CoursesController : ApiController
    {
        private BaseService<Course> service;
        private UnitOfWork unitOfWork = new UnitOfWork();

        public CoursesController()
        {
            service = new BaseService<Course>(new ModelStateWrapper(this.ModelState), unitOfWork);
        }

        public CoursesController(BaseService<Course> service)
        {
            this.service = service;
        }

        private LearnToLearnContext db = new LearnToLearnContext();

        [Route("")]
        [HttpGet]
        public List<AllCoursesBindModel> GetAll()
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
            if (token == null || token.EndDate < DateTime.Now)
                {
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
                else
                {
                    List<Course> courses = new List<Course>();
                    if (token.User.IsTeacher == true)
                    {
                        courses = service.GetAll();
                    }
                    else
                    {
                        courses = service.GetAll(c => c.isVisible == true);
                    }
                    List<AllCoursesBindModel> courseBindModel = new List<AllCoursesBindModel>();

                    foreach (var course in courses)
                    {
                        AllCoursesBindModel bindModel = new AllCoursesBindModel
                        {
                            Name = course.Name,
                            Description = course.Description,
                            TeacherId = course.TeacherId,
                            Capacity = course.Capacity,
                            CreatedAt = course.CreatedAt,
                            UpdatedAt = course.UpdatedAt
                        };
                        courseBindModel.Add(bindModel);
                    }
                    return courseBindModel;
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
                if (!CourseExists(id))
                {
                    var course = service.GetById(id);
                    if (course.isVisible == false && token.User.IsTeacher == false)
                    {
                        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                        message.Content = new StringContent("You don't have permission to see this course!");
                        throw new HttpResponseException(message);
                    }
                    if (course == null)
                    {
                        return NotFound();
                    }
                    AllCoursesBindModel bindModel = new AllCoursesBindModel
                    {
                        Name = course.Name,
                        Description = course.Description,
                        TeacherId = course.TeacherId,
                        Capacity = course.Capacity,
                        CreatedAt = course.CreatedAt,
                        UpdatedAt = course.UpdatedAt
                    };
                    return Ok(bindModel);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("The course doesn't exist!");
                    throw new HttpResponseException(message);
                }
            }
        }

        [Route("{Id}")]
        [HttpPut]
        public IHttpActionResult PutCourse(int id, [FromBody] PutCourseBindModel courseBindModel)
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
                if (!CourseExists(id))
                {
                    Course course = new Course
                    {
                        Id = id,
                        Name = courseBindModel.Name,
                        Description = courseBindModel.Description,
                        TeacherId = courseBindModel.TeacherId,
                        Capacity = courseBindModel.Capacity,
                        isVisible = courseBindModel.IsVisible,
                        UpdatedAt = DateTime.Now
                    };

                    service.Update(course);
                    service.Save();
                    return Ok(courseBindModel);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("The course doesn't exist!");
                    throw new HttpResponseException(message);
                }
            }
        }

        [Route("", Name = "Action")]
        [HttpPost]
        public IHttpActionResult PostCourse([FromBody] AllCoursesBindModel courseBindModel)
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
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    Course course = new Course
                    {
                        Name = courseBindModel.Name,
                        Description = courseBindModel.Description,
                        TeacherId = courseBindModel.TeacherId,
                        Capacity = courseBindModel.Capacity,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        isVisible = courseBindModel.IsVisible
                    };
                    courseBindModel.CreatedAt = course.CreatedAt;
                    courseBindModel.UpdatedAt = course.UpdatedAt;
                    service.Create(course);
                    service.Save();

                    return CreatedAtRoute("Action", new { id = course.Id }, courseBindModel);
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("You don't have permission for this activity!");
                    throw new HttpResponseException(message);
                }
        }

        [Route("{Id}")]
        [HttpDelete]
        public IHttpActionResult DeleteCourse(int id)
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
                if (!CourseExists(id))
                {
                    BaseService<Enrollment> enrollmentService = new BaseService<Enrollment>(new ModelStateWrapper(this.ModelState), unitOfWork);
                    List<Enrollment> enrollments = enrollmentService.GetAll();
                    foreach (var item in enrollments)
                    {
                        if (item.CourseId == id)
                        {
                            enrollmentService.Delete(item.Id);
                            enrollmentService.Save();
                        }
                    }
                    service.Delete(id);
                    service.Save();
                    return Ok();
                }
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                    message.Content = new StringContent("The courses you want to delete doesn't exist");
                    throw new HttpResponseException(message);
                }
            }
        }

        private bool CourseExists(int id)
        {
            bool flag = false;
            List<Course> courses = service.GetAll();
            foreach (var user in courses)
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
