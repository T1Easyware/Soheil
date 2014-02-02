using System.Globalization;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;

using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class PositionAccessRuleVM : ItemRelationDetailViewModel
    {
        private readonly Position_AccessRule _model;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public PositionAccessRuleVM(AccessType access, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="presentationType"></param>
        public PositionAccessRuleVM(Position_AccessRule entity, AccessType access, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData();
            _model = entity;
            PositionId = entity.Position.Id;
            AccessRuleId = entity.AccessRule.Id;
            PositionName = entity.Position.Name;
            PositionCode = entity.Position.Id.ToString(CultureInfo.InvariantCulture);
            AccessRuleName = entity.AccessRule.Name;
            AccessRuleCode = entity.AccessRule.Code;
            if (entity.Type != null) Type = (AccessType)entity.Type;
        }

        private void InitializeData()
        {
            //DataService = new PositionAccessRuleDataService();
            SaveCommand = new Command(Save, CanSave);
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(PositionAccessRulesVM), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return _model.Id; }
            set { }
        }

        public int PositionId { get; set; }
        public int AccessRuleId { get; set; }
        public string PositionName { get; set; }
        public string PositionCode { get; set; }
        public string AccessRuleName { get; set; }
        public string AccessRuleCode { get; set; }
        public AccessType Type { get; set; }
      
    }
}