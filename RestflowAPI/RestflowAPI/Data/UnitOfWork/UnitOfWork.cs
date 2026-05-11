using Microsoft.EntityFrameworkCore;
using RestflowAPI.Repositories;
using RestflowAPI.Repositories.Interfaces;
using System;
using System.Collections;

namespace RestflowAPI.Data.UnitOfWork
{
	public class UnitOfWork: IUnitOfWork
	{
		private readonly ApplicationDbContext _dbContext;
        public UnitOfWork(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
		{
			return await _dbContext.SaveChangesAsync(cancellationToken);
		}

        
    }
}
