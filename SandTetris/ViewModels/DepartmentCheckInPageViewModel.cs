using CommunityToolkit.Mvvm.ComponentModel;
using SandTetris.Entities;
using SandTetris.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandTetris.ViewModels;

public partial class DepartmentCheckInPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    ObservableCollection<Department> departments = new ObservableCollection<Department>();

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {

    }

    public DepartmentCheckInPageViewModel(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    private readonly IDepartmentRepository _departmentRepository;
}
