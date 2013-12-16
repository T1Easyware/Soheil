using System.Globalization;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;

using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class UserPositionVM : ItemRelationDetailViewModel
    {
        private readonly User_Position _model;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public UserPositionVM(AccessType access, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="presentationType"></param>
        public UserPositionVM(User_Position entity, AccessType access, RelationDirection presentationType):base(access, presentationType)
        {
            _model = entity;
            InitializeData();
            UserId = entity.User.Id;
            PositionId = entity.Position.Id;
            UserName = entity.User.Username;
            UserCode = entity.User.Code.ToString(CultureInfo.InvariantCulture);
            PositionName = entity.Position.Name;
            PositionCode = entity.Position.Id.ToString(CultureInfo.InvariantCulture);
        }

        private void InitializeData()
        {
            SaveCommand = new Command(Save, CanSave);
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(UserPositionsVM), new PropertyMetadata(0));

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
        public int PositionId { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string PositionName { get; set; }
        public string PositionCode { get; set; }

    }
}