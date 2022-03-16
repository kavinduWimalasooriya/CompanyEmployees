using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class EmployeeService :IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeeService(IRepositoryManager repository,ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employee, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId,trackChanges:false);
            if (company == null) throw new CompanyNotFoundException(companyId);
            var employeeEntity = _mapper.Map<Employee>(employee);
            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsAsync();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeToReturn;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company == null) throw new CompanyNotFoundException(companyId);
            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employee == null) throw new EmployeeNotFoundException(id);
            _repository.Employee.DeleteEmployee(employee);
            await _repository.SaveAsAsync();
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId,trackChanges:false);
            if (company == null)
                throw new CompanyNotFoundException(companyId);
            var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId,id,trackChanges:false);
            if (employeeFromDb == null)
                throw new EmployeeNotFoundException(id);
            var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);
            return employeeDto;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
                throw new CompanyNotFoundException(companyId);
            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            return employeesDto;
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, 
            bool compTrackChanges, bool empTrackChanges)
        {
            var comapny = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges);
            if (comapny == null) throw new CompanyNotFoundException(companyId);
            var employee = await _repository.Employee.GetEmployeeAsync(companyId,id,empTrackChanges);
            if (employee == null) throw new EmployeeNotFoundException(id);
            _mapper.Map(employeeForUpdate, employee);
            await _repository.SaveAsAsync();
        }
    }
}
