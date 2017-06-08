using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Domain;

namespace API.Controllers
{
    public class PredictionsController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Predictions
        public IQueryable<Prediction> GetPredictions()
        {
            return db.Predictions;
        }

        // GET: api/Predictions/5
        [ResponseType(typeof(Prediction))]
        public async Task<IHttpActionResult> GetPrediction(int id)
        {
            Prediction prediction = await db.Predictions.FindAsync(id);
            if (prediction == null)
            {
                return NotFound();
            }

            return Ok(prediction);
        }

        // PUT: api/Predictions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPrediction(int id, Prediction prediction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != prediction.PredictionId)
            {
                return BadRequest();
            }

            db.Entry(prediction).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PredictionExists(id))
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

        // POST: api/Predictions
        [ResponseType(typeof(Prediction))]
        public async Task<IHttpActionResult> PostPrediction(Prediction prediction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // **
            var match = await db.Matches.FindAsync(prediction.MatchId);

            if (match == null)
            {
                return BadRequest("The match doesn't exits.");
            }

            if (match.DateTime < DateTime.Now.AddHours(-5))
            {
                return BadRequest("The match start, you dont do the prediction.");
            }

            var oldPrediction = await db.Predictions
                .Where(p => p.MatchId == prediction.MatchId &&
                            p.UserId == prediction.UserId)
                .FirstOrDefaultAsync();

            if (oldPrediction == null)
            {
                db.Predictions.Add(prediction);
            }
            else
            {
                oldPrediction.LocalGoals = prediction.LocalGoals;
                oldPrediction.VisitorGoals = prediction.VisitorGoals;
                db.Entry(oldPrediction).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();
            return Ok(prediction);
        }

        // DELETE: api/Predictions/5
        [ResponseType(typeof(Prediction))]
        public async Task<IHttpActionResult> DeletePrediction(int id)
        {
            Prediction prediction = await db.Predictions.FindAsync(id);
            if (prediction == null)
            {
                return NotFound();
            }

            db.Predictions.Remove(prediction);
            await db.SaveChangesAsync();

            return Ok(prediction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PredictionExists(int id)
        {
            return db.Predictions.Count(e => e.PredictionId == id) > 0;
        }
    }
}