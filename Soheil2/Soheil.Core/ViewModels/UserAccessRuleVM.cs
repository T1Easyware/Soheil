using System.Globalization;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class UserAccessRuleVM : ItemRelationDetailViewModel
    {
        private readonly User_AccessRule _model;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public UserAccessRuleVM(AccessType access, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="presentationType"></param>
        public UserAccessRuleVM(User_AccessRule entity, AccessType access, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData();
            _model = entity;
            UserId = entity.User.Id;
            AccessRuleId = entity.AccessRule.Id;
            UserName = entity.User.Username;
            UserCode = entity.User.Code.ToString(CultureInfo.InvariantCulture);
            AccessRuleName = entity.AccessRule.Name;
            AccessRuleCode = entity.AccessRule.Code;
            if (entity.Type != null) Type = (AccessType) entity.Type;
        }

        private void InitializeData()
        {
            SaveCommand = new Command(Save, CanSave);
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(UserAccessRulesVM), new PropertyMetadata(0));

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

        public int UserId { get; set; }
        public int AccessRuleId { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string AccessRuleName { get; set; }
        public string AccessRuleCode { get; set; }
        public AccessType Type { get; set; }

    }
}