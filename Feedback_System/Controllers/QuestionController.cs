using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;

namespace Feedback_System.Controllers
{
    [Route("api/QuestionApi")]
    public class QuestionController:Controller
    {

        private readonly ApplicationDBContext _db;

        public QuestionController(ApplicationDBContext db)
        {
            _db = db;
        }


        // GET: api/Studentapi
        [Route("GetQuetions")]
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<FeedbackQuestionsDto>> GetQuetions()
        {
            try
            {
                var FeedbackQuetions = _db.FeedbackQuetions
                    .Select(q => new FeedbackQuestionsDto
                    {
                        question_id = q.question_id,
                        question = q.question,
                        question_type = q.question_type,
                        feedback_type_id = q.feedback_type_id,
                    })
                    .ToList();

                return Ok(FeedbackQuetions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetQutionsByQuestionid/{question_id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<FeedbackQuestionsDto> GetQuestionsbyid(int question_id)
        {
            //if (string.IsNullOrEmpty(student_rollno))
            //    return BadRequest("Student roll number is required.");

            var FeedbackQutions = _db.FeedbackQuetions.FirstOrDefault(u => u.question_id == question_id);

            if (FeedbackQutions == null)
                return NotFound($"No question found with this id '{question_id}'.");

            var FeedbackQutionsDto = new FeedbackQuestionsDto
            {
                question_id = FeedbackQutions.question_id,
                question = FeedbackQutions.question,
                question_type = FeedbackQutions.question_type,
                feedback_type_id = FeedbackQutions.feedback_type_id,
            };

            return Ok(FeedbackQutionsDto);
        }




        // POST: api/Studentapi
        [Route("CreateQutions")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult<FeedbackQuestionsDto> CreateQutions([FromBody] FeedbackQuestionsDto feedbackqutionsdto)
        {

            if (_db.FeedbackQuetions.Any(u => u.question_id == feedbackqutionsdto.question_id))
            {
                ModelState.AddModelError("question_id", "A qutions with this qutions id is already exists.");
                return BadRequest(ModelState);
            }
            var FeedbackQutions = new FeedbackQuetions
            {
                question_id = feedbackqutionsdto.question_id,
                question = feedbackqutionsdto.question,
                question_type = feedbackqutionsdto.question_type,
                feedback_type_id = feedbackqutionsdto.feedback_type_id,
            };

            _db.FeedbackQuetions.Add(FeedbackQutions);
            _db.SaveChanges();

            var createdDto = new FeedbackQuestionsDto
            {
                question_id = feedbackqutionsdto.question_id,
                question = feedbackqutionsdto.question,
                question_type = feedbackqutionsdto.question_type,
                feedback_type_id = feedbackqutionsdto.feedback_type_id,
            };

            return CreatedAtAction(nameof(GetQuetions),
                new { student_rollno = createdDto.question_id },
                createdDto);
        }
        [HttpPut("UpdateQuetions")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateQuetions([FromBody] FeedbackQuestionsDto feedbackqutionsdto)
        {
            if (feedbackqutionsdto == null)
                return BadRequest("Invalid question data.");

            var questionsFromDb = _db.FeedbackQuetions.FirstOrDefault(u => u.question_id == feedbackqutionsdto.question_id);
            if (questionsFromDb == null)
                return NotFound($"No questions found with this question id '{feedbackqutionsdto.question_id}'.");

            questionsFromDb.question_id = feedbackqutionsdto.question_id;
            questionsFromDb.question = feedbackqutionsdto.question;
            questionsFromDb.question_type = feedbackqutionsdto.question_type;
            questionsFromDb.feedback_type_id = feedbackqutionsdto.feedback_type_id;

            _db.FeedbackQuetions.Update(questionsFromDb);
            _db.SaveChanges();
            return Ok(questionsFromDb);
        }


        [HttpDelete("DeleteQuestions/{question_id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteQuestions(int question_id)
        {
            //if (string.IsNullOrWhiteSpace(student_rollno))
            //    return BadRequest("Student roll number is required.");

            var FeedbackQutions = _db.FeedbackQuetions.FirstOrDefault(u => u.question_id == question_id);
            if (FeedbackQutions == null)
                return NotFound($"No questions found with this id '{question_id}'.");

            _db.FeedbackQuetions.Remove(FeedbackQutions);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
