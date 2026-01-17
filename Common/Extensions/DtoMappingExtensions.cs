using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Common.Extensions
{
    public static class DtoMappingExtensions
    {

        #region Photos

        public static EntityPhoto ToEFModel(this PhotoDto dto)
        {
            return new EntityPhoto
            {
                Id = dto.Id,
                Url = dto.Url,
                Type = dto.Type,
                IsMain = dto.IsMain,
                TelegramPhoto = dto.TelegramPhoto?.ToEFModel()
            };
        }

        public static PhotoDto ToDto(this EntityPhoto entity)
        {
            return new PhotoDto(
                Id: entity.Id,
                Url: entity.Url,
                Type: entity.Type,
                IsMain: entity.IsMain,
                TelegramPhoto: entity.TelegramPhoto?.ToDto()
            );
        }

        public static TelegramPhoto ToEFModel(this TelegramPhotoDto dto)
        {
            return new TelegramPhoto
            {
                Id = dto.Id,
                FileId = dto.FileId
            };
        }

        public static TelegramPhotoDto ToDto(this TelegramPhoto entity)
        {
            return new TelegramPhotoDto(
                Id: entity.Id,
                FileId: entity.FileId
            );
        }

        #endregion

        #region CatColor

        public static CatColorDetailDto ToDetailDto(this CatColor entity)
        {
            return new CatColorDetailDto(
                Id: entity.Id,
                Identifier: entity.Identifier,
                Description: entity.Description,
                IsEnabled: entity.IsEnabled,
                Photos: [.. entity.Photos.Select(p => p.ToDto())]
            );
        }

        public static CatColorListDto ToListDto(this CatColor entity)
        {
            return new CatColorListDto(
                Id: entity.Id,
                Identifier: entity.Identifier,
                Description: entity.Description,
                IsEnabled: entity.IsEnabled,
                MainPhotoUrl: entity.Photos?
                    .Where(p => p.Type == PhotosType.Photos)
                    .OrderByDescending(p => p.IsMain)
                    .ThenBy(p => p.Id)
                    .Select(p => p.Url)
                    .FirstOrDefault()
            );
        }

        public static CatColor ToEFModel(this CatColorDetailDto dto)
        {
            return new CatColor
            {
                Id = dto.Id,
                Identifier = dto.Identifier,
                Description = dto.Description,
                IsEnabled = dto.IsEnabled,
                Photos = [.. dto.Photos.Select(p => p.ToEFModel())]
            };
        }

        public static CatColor ToEFModel(this CreateCatColorDto dto)
        {
            return new CatColor
            {
                Identifier = dto.Identifier,
                Description = dto.Description
            };
        }

        public static UpdateCatColorDto ToUpdateDto(this CatColor model)
        {
            return new UpdateCatColorDto(
                Identifier: model.Identifier,
                Description: model.Description,
                Enabled: model.IsEnabled
            );
        }

        public static CatColor ToEFModel(this CatColorListDto dto)
        {
            return new CatColor
            {
                Id = dto.Id,
                Identifier = dto.Identifier,
                Description = dto.Description,
                IsEnabled = dto.IsEnabled
            };
        }

        public static void ApplyUpdate(this CatColor catColor, UpdateCatColorDto dto)
        {
            catColor.Identifier = dto.Identifier;
            catColor.Description = dto.Description;
            catColor.IsEnabled = dto.Enabled;
        }

        #endregion

        #region ParentCat

        public static ParentCatMinimalDto ToMinimalDto(this ParentCat entity)
        {
            return new ParentCatMinimalDto(
                Id: entity.Id,
                Name: entity.Name,
                IsEnabled: entity.IsEnabled
            );
        }

        public static ParentCatDetailDto ToDetailDto(this ParentCat entity)
        {
            return new ParentCatDetailDto(
                Id: entity.Id,
                Name: entity.Name,
                BirthDay: entity.BirthDay,
                IsMale: entity.IsMale,
                Color: entity.Color,
                Description: entity.Description,
                IsEnabled: entity.IsEnabled,
                Photos: [.. entity.Photos.Select(p => p.ToDto())]
            );
        }

        public static ParentCatListDto ToListDto(this ParentCat entity)
        {
            return new ParentCatListDto(
                Id: entity.Id,
                Name: entity.Name,
                BirthDay: entity.BirthDay,
                IsMale: entity.IsMale,
                Color: entity.Color,
                Description: entity.Description,
                IsEnabled: entity.IsEnabled,
                MainPhotoUrl: entity.Photos?
                    .Where(p => p.Type == PhotosType.Photos)
                    .OrderByDescending(p => p.IsMain)
                    .ThenBy(p => p.Id)
                    .Select(p => p.Url)
                    .FirstOrDefault()
            );
        }

        public static ParentCat ToEFModel(this ParentCatMinimalDto dto)
        {
            return new ParentCat
            {
                Id = dto.Id,
                Name = dto.Name,
                IsEnabled = dto.IsEnabled,
            };
        }

        public static ParentCat ToEFModel(this CreateParentCatDto dto)
        {
            return new ParentCat
            {
                Name = dto.Name,
                BirthDay = dto.BirthDay,
                IsMale = dto.IsMale,
                Description = dto.Description,
                Color = dto.Color,
                IsEnabled = false
            };
        }

        public static ParentCat ToEFModel(this ParentCatDetailDto dto)
        {
            return new ParentCat
            {
                Id = dto.Id,
                Name = dto.Name,
                BirthDay = dto.BirthDay,
                IsMale = dto.IsMale,
                Description = dto.Description,
                Color = dto.Color,
                IsEnabled = dto.IsEnabled,
                Photos = [.. dto.Photos.Select(p => p.ToEFModel())]
            };
        }

        public static void ApplyUpdate(this ParentCat entity, UpdateParentCatDto dto)
        {
            entity.Name = dto.Name;
            entity.BirthDay = dto.BirthDay;
            entity.IsMale = dto.IsMale;
            entity.IsEnabled = dto.IsEnabled;
            entity.Description = dto.Description;
            entity.Color = dto.Color;
        }

        public static UpdateParentCatDto ToUpdateDto(this ParentCat model)
        {
            return new UpdateParentCatDto(
                Name: model.Name,
                BirthDay: model.BirthDay,
                IsMale: model.IsMale,
                IsEnabled: model.IsEnabled,
                Description: model.Description,
                Color: model.Color
            );
        }

        #endregion

        #region Kitten

        public static KittenMinimalDto ToMinimalDto(this Kitten entity)
        {
            return new KittenMinimalDto(
                Id: entity.Id,
                Name: entity.Name,
                IsEnabled: entity.IsEnabled
            );
        }

        public static KittenDetailDto ToDetailDto(this Kitten entity)
        {
            return new KittenDetailDto(
                Id: entity.Id,
                Name: entity.Name,
                BirthDay: entity.BirthDay,
                IsMale: entity.IsMale,
                IsEnabled: entity.IsEnabled,
                Description: entity.Description,
                Color: entity.Color,
                Class: entity.Class,
                Status: entity.Status,
                Photos: [.. entity.Photos.Select(p => p.ToDto())],
                Litter: entity.Litter.ToMinimalDto()
            );
        }

        public static KittenListDto ToListDto(this Kitten entity)
        {
            return new KittenListDto(
                Id: entity.Id,
                Name: entity.Name,
                BirthDay: entity.BirthDay,
                MainPhotoUrl: entity.Photos?
                    .Where(p => p.Type == PhotosType.Photos)
                    .OrderByDescending(p => p.IsMain)
                    .ThenBy(p => p.Id)
                    .Select(p => p.Url)
                    .FirstOrDefault(),
                Description: entity.Description,
                Status: entity.Status,
                Color: entity.Color,
                IsMale: entity.IsMale,
                IsEnabled: entity.IsEnabled,
                LitterLetter: entity.Litter?.Letter ?? '?',
                LitterId: entity.LitterId
            );
        }

        public static Kitten ToEFModel(this CreateKittenDto dto, DateOnly litterBirthDay)
        {
            return new Kitten
            {
                LitterId = dto.LitterId,
                Name = dto.Name,
                IsMale = dto.IsMale,
                Description = dto.Description,
                Color = dto.Color,
                Class = dto.Class,
                Status = dto.Status,
                BirthDay = litterBirthDay
            };
        }

        public static Kitten ToEFModel(this KittenDetailDto dto)
        {
            return new Kitten
            {
                Id = dto.Id,
                LitterId = dto.Litter.Id,
                Name = dto.Name,
                BirthDay = dto.BirthDay,
                IsMale = dto.IsMale,
                IsEnabled = dto.IsEnabled,
                Description = dto.Description,
                Color = dto.Color,
                Class = dto.Class,
                Status = dto.Status,
                Photos = [.. dto.Photos.Select(p => p.ToEFModel())]
            };
        }

        public static Kitten ToEFModel(this KittenListDto dto)
        {
            return new Kitten
            {
                Id = dto.Id,
                Name = dto.Name,
                BirthDay = dto.BirthDay,
                IsMale = dto.IsMale,
                Color = dto.Color,
                Status = dto.Status,
                LitterId = dto.LitterId,
                IsEnabled = dto.IsEnabled,
                Description = dto.Description
            };
        }

        public static Kitten ToEFModel(this KittenMinimalDto dto)
        {
            return new Kitten
            {
                Id = dto.Id,
                Name = dto.Name,
                IsEnabled = dto.IsEnabled,
            };
        }

        public static void ApplyUpdate(this Kitten entity, UpdateKittenDto dto)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.IsMale = dto.IsMale;
            entity.Color = dto.Color;
            entity.Class = dto.Class;
            entity.Status = dto.Status;
        }

        public static UpdateKittenDto ToUpdateDto(this Kitten model)
        {
            return new UpdateKittenDto(
                Name: model.Name,
                Description: model.Description,
                IsMale: model.IsMale,
                Color: model.Color,
                Class: model.Class,
                Status: model.Status,
                IsEnabled: model.IsEnabled
            );
        }

        #endregion

        #region Litter

        public static LitterMinimalDto ToMinimalDto(this Litter entity)
        {
            return new LitterMinimalDto(
                Id: entity.Id,
                Letter: entity.Letter,
                IsEnabled: entity.IsEnabled
            );
        }

        public static LitterSimpleDto ToSimpleDto(this Litter entity)
        {
            return new LitterSimpleDto(
                Id: entity.Id,
                Letter: entity.Letter,
                IsEnabled: entity.IsEnabled,
                MotherName: entity.MotherCat?.Name,
                FatherName: entity.FatherCat?.Name
            );
        }

        public static LitterDetailDto ToDetailDto(this Litter entity)
        {
            return new LitterDetailDto(
                Id: entity.Id,
                Letter: entity.Letter,
                BirthDay: entity.BirthDay,
                IsEnabled: entity.IsEnabled,
                Description: entity.Description,
                Photos: [.. entity.Photos.Select(p => p.ToDto())],
                MotherCatId: entity.MotherCatId,
                MotherCat: entity.MotherCat?.ToMinimalDto(),
                FatherCatId: entity.FatherCatId,
                FatherCat: entity.FatherCat?.ToMinimalDto(),
                Kittens: [.. entity.Kittens.Select(k => k.ToListDto())],
                TotalKittens: entity.Kittens.Count,
                AvailableKittens: entity.Kittens.Count(k => k.Status == KittenStatus.Available)
            );
        }

        public static Litter ToEFModel(this CreateLitterDto dto, char letter)
        {
            return new Litter
            {
                BirthDay = dto.BirthDay,
                Description = dto.Description,
                MotherCatId = dto.MotherCatId,
                FatherCatId = dto.FatherCatId,
                Letter = letter,
                IsEnabled = false
            };
        }

        public static Litter ToEFModel(this LitterDetailDto dto)
        {
            return new Litter
            {
                Id = dto.Id,
                Letter = dto.Letter,
                Description = dto.Description,
                BirthDay = dto.BirthDay,
                FatherCat = dto.FatherCat?.ToEFModel(),
                FatherCatId = dto.FatherCatId,
                MotherCat = dto.MotherCat?.ToEFModel(),
                MotherCatId = dto.MotherCatId,
                IsEnabled = dto.IsEnabled,
                Photos = [.. dto.Photos.Select(p => p.ToEFModel())],
                Kittens = dto.Kittens?.Select(k => k.ToEFModel()).ToList() ?? []
            };
        }

        public static Litter ToEFModel(this LitterSimpleDto dto)
        {
            return new Litter
            {
                Id = dto.Id,
                Letter = dto.Letter,
                IsEnabled = dto.IsEnabled
            };
        }

        public static Litter ToEFModel(this LitterMinimalDto dto)
        {
            return new Litter
            {
                Id = dto.Id,
                Letter = dto.Letter,
                IsEnabled = dto.IsEnabled,
            };
        }

        public static void ApplyUpdate(this Litter entity, UpdateLitterDto dto)
        {
            entity.Letter = dto.Letter;
            entity.BirthDay = dto.BirthDay;
            entity.IsEnabled = dto.IsEnabled;
            entity.Description = dto.Description;
            entity.MotherCatId = dto.MotherCatId;
            entity.FatherCatId = dto.FatherCatId;
        }

        public static UpdateLitterDto ToUpdateDto(this Litter model)
        {
            return new UpdateLitterDto(
                Letter: model.Letter,
                BirthDay: model.BirthDay,
                IsEnabled: model.IsEnabled,
                Description: model.Description,
                MotherCatId: model.MotherCatId,
                FatherCatId: model.FatherCatId
            );
        }

        #endregion

        #region BookingRequests

        public static BookingRequestDetailDto ToDetailDto(this BookingRequest model)
        {
            return new BookingRequestDetailDto(
                Id: model.Id,
                CustomerName: model.CustomerName,
                CustomerPhone: model.CustomerPhone,
                IsProcessed: model.IsProcessed,
                CuratorId: model.CuratorTelegramId,
                KittenId: model.KittenId);
        }

        public static BookingRequest ToEFModel(this CreateBookingRequestDto dto)
        {
            return new BookingRequest
            {
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                KittenId = dto.KittenId
            };
        }

        public static BookingRequest ToEFModel(this BookingRequestDetailDto dto)
        {
            return new BookingRequest
            {
                Id = dto.Id,
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                KittenId = dto.KittenId,
                CuratorTelegramId = dto.CuratorId,
                IsProcessed = dto.IsProcessed,
            };
        }

        #endregion

    }
}
