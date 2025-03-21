using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Newtonsoft.Json;

namespace BlueBellDolls.Bot.Providers
{
    public class CallbackDataProvider : ICallbackDataProvider
    {
        public string CreateConfirmCallback(string baseCallback)
            => $"confirm_{baseCallback}";

        public string CreateEditEntityCallback(IDisplayableEntity entity)
            => $"edit{entity.GetType().Name}-{entity.Id}";

        public string CreateSelectParentCatCallback(bool isMale, int litterId)
            => $"selectParentCat-{isMale}-{litterId}";

        public string CreateOpenEntityInLitterCallback(IEntity entity, int litterId)
            => $"open{entity.GetType().Name}-{litterId}-{entity.Id}";

        public string CreateAddKittenToLitterCallback(int litterId)
            => $"addKittenToLitter-{litterId}";

        public string CreateBackToLitterCallback(int litterId)
            => $"edit{nameof(Litter)}-{litterId}";

        public string CreateListEntityCallback(string entityName, int page)
            => $"list{entityName}-{page}";

        public string CreateDeleteEntityCallback(IDisplayableEntity entity, int fromLitterId = 0)
            => $"delete{entity.GetType().Name}{(fromLitterId != 0 ? $"-fromLitter-{fromLitterId}" : string.Empty)}-{entity.Id}";

        public string CreateDeleteParentCatFromLitterCallback(int parentCatId, int litterId)
            => $"deleteFromLitter-{litterId}-{parentCatId}";

        public string CreateEntityReferenceCallback(IDisplayableEntity entity, ListUnitActionMode actionMode, IEntity? unitOwner = null)
        {
            string ownerCallbackData = string.Empty;
            if (unitOwner != null)
            {
                if (actionMode != ListUnitActionMode.Select)
                    throw new ArgumentException("При указанном владельце сущности режим работы может быть только Select!");
                ownerCallbackData = $"To{unitOwner.GetType().Name}-{unitOwner.Id}-";
            }
            return $"{actionMode.ToString().ToLower()}{ownerCallbackData}{entity.GetType().Name}-{entity.Id}";
        }

        public string CreateAddEntityCallback(string entityName)
            => $"add{entityName}";

        public string CreateAddPhotosCallback(IDisplayableEntity entity)
            => $"managePhotosTo{entity.GetType().Name}-{entity.Id}";

        public string CreateAddTitlesCallback(IDisplayableEntity entity)
            => $"manageTitlesTo{entity.GetType().Name}-{entity.Id}";

        public string CreateTogglePhotoSelectionCallback(IDisplayableEntity entity, int number, bool select) 
            => $"togglePhotoFor{entity.GetType().Name}-{number}-{select}-{entity.Id}";

        public string CreateMakeDefaultPhotoForEntityCallback(IDisplayableEntity entity, int photoIndex)
            => $"setDefaultPhotoFor{entity.GetType().Name}-{photoIndex}-{entity.Id}";

        public string CreateDeletePhotosForEntityCallback(IDisplayableEntity entity)
            => $"deletePhotosFor{entity.GetType().Name}-{entity.Id}";

        public string CreateDeleteMessagesCallback(int[] messagesId)
            => $"dm-[{string.Join(", ", messagesId)}]";
    }
}
