using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
	public class GeneralSkillVM : ItemContentViewModel
	{
		#region Properties

		private PersonalSkill _model;

		public override int Id
		{
			get { return _model.Id; }
			set { }
		}

		public override string SearchItem { get { return Education + Reserve1 + Reserve2; } set { } }

		public int Experience
		{
			get { return _model.Experience; }
			set { _model.Experience = value; OnPropertyChanged("Experience"); }
		}

		/// <summary>
		/// Gets or sets the data service.
		/// </summary>
		/// <value>
		/// The data service.
		/// </value>
		public GeneralSkillDataService GeneralSkillDataService { get; set; }

		public string Education
		{
			get { return _model.Education; }
			set { _model.Education = value; OnPropertyChanged("Education"); }
		}

		public int Reserve1
		{
			get { return _model.ReserveInteger1; }
			set { _model.ReserveInteger1 = value; OnPropertyChanged("Reserve1"); }
		}

		public string Reserve2
		{
			get { return _model.ReserveText1; }
			set { _model.ReserveText1 = value; OnPropertyChanged("Reserve2"); }
		}

		public string Reserve3
		{
			get { return _model.ReserveText2; }
			set { _model.ReserveText2 = value; OnPropertyChanged("Reserve3"); }
		}

		public string ModifiedBy
		{
			get { return LoginInfo.GetUsername(_model.ModifiedBy); }

		}

		#endregion

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
		/// </summary>
		public GeneralSkillVM(AccessType access, GeneralSkillDataService dataService)
			: base(access)
		{
			InitializeData(dataService);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
		/// </summary>
		/// <param name="entity">The model.</param>
		/// <param name="access"></param>
		/// <param name="dataService"></param>
		public GeneralSkillVM(PersonalSkill entity, AccessType access, GeneralSkillDataService dataService)
			: base(access)
		{
			InitializeData(dataService);
			_model = entity;
		}

		private void InitializeData(GeneralSkillDataService dataService)
		{
			GeneralSkillDataService = dataService;
			SaveCommand = new Command(Save, CanSave);
		}

		public override void Save(object param)
		{
			GeneralSkillDataService.AttachModel(_model);
			_model = GeneralSkillDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy"); OnPropertyChanged("ModifiedDate"); Mode = ModificationStatus.Saved;
		}

		public override bool CanSave()
		{
			return AllDataValid() && base.CanSave();
		}

		#endregion

		#region Static Methods
		public static PersonalSkill CreateNew(GeneralSkillDataService dataService)
		{
			int id = dataService.AddModel(new PersonalSkill { Experience = 0, Education = "دیپلم" });
			return dataService.GetSingle(id);
		}
		#endregion
	}
}