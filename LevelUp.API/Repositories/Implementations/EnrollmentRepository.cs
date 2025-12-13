using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;

namespace LevelUp.API.Repositories.Implementations;

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(LevelUpDbContext context) : base(context)
    {
    }

}
