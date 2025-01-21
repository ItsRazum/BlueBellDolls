using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Enums;
using System.Globalization;

namespace BlueBellDolls.Service.Grpc
{
    public static class GrpcExtensions
    {

        #region Fields

        private static readonly CultureInfo _cultureInfo = new("ru-RU");

        #endregion

        #region BlueBellDollsService ParentCat <-> Protobuf ParentCat

        public static BlueBellDolls.Grpc.ParentCat Compress(this ParentCat parentCat)
        {
            var result = new BlueBellDolls.Grpc.ParentCat
            {
                Id = parentCat.Id,
                Name = parentCat.Name,
                IsMale = parentCat.IsMale,
                Description = parentCat.Description,
                OldDescription = parentCat.OldDescription,
                GeneticTestOne = parentCat.GeneticTestOne,
                GeneticTestTwo = parentCat.GeneticTestTwo,
                BirthDay = parentCat.BirthDay.ToString(_cultureInfo),
            };

            result.Photos.AddRange(parentCat.Photos);
            result.Titles.AddRange(parentCat.Titles);

            return result;
        }

        public static ParentCat Decompress(this BlueBellDolls.Grpc.ParentCat parentCat)
        {
            return new ParentCat
            {
                Id = parentCat.Id,
                Name = parentCat.Name,
                IsMale = parentCat.IsMale!.Value,
                Description = parentCat.Description,
                OldDescription = parentCat.OldDescription,
                GeneticTestOne = parentCat.GeneticTestOne,
                GeneticTestTwo = parentCat.GeneticTestTwo,
                Photos = new List<string>(parentCat.Photos),
                Titles = new List<string>(parentCat.Titles)
            };
        }

        #endregion

        #region BlueBellDollsService Kitten <-> Protobuf Kitten

        public static BlueBellDolls.Grpc.Kitten Compress(this Kitten kitten)
        {
            var result = new BlueBellDolls.Grpc.Kitten
            {
                Id = kitten.Id,
                Name = kitten.Name,
                IsMale = kitten.IsMale,
                BirthDay = kitten.BirthDay.ToString(_cultureInfo),
                Class = kitten.Class.ToString(),
                Status = kitten.Status.ToString(),
                Description = kitten.Description
            };

            result.Photos.AddRange(kitten.Photos);

            return result;
        }

        public static Kitten Decompress(this BlueBellDolls.Grpc.Kitten kitten)
        {
            return new Kitten
            {
                Id = kitten.Id,
                Name = kitten.Name,
                IsMale = kitten.IsMale!.Value,
                BirthDay = DateOnly.Parse(kitten.BirthDay, _cultureInfo),
                Class = Enum.Parse<KittenClass>(kitten.Class),
                Status = Enum.Parse<KittenStatus>(kitten.Status),
                Description = kitten.Description,
                Photos = new List<string>(kitten.Photos)
            };
        }

        #endregion

        #region BlueBellDollsService Litter <-> Protobuf Litter

        public static BlueBellDolls.Grpc.Litter Compress(this Common.Models.Litter litter)
        {
            var result = new BlueBellDolls.Grpc.Litter
            {
                Id = litter.Id,
                Letter = litter.Letter.ToString(),
                BirthDay = litter.BirthDay.ToString(_cultureInfo),
                IsActive = litter.IsActive,
                Description = litter.Description,
                MotherCat = litter.MotherCat.Compress(),
                FatherCat = litter.FatherCat.Compress()
            };

            foreach (var kitten in litter.Kittens)
                result.Kittens.Add(kitten.Compress());

            return result;
        }

        public static Litter Decompress(this BlueBellDolls.Grpc.Litter litter)
        {
            var result = new Litter
            {
                Id = litter.Id,
                Letter = char.Parse(litter.Letter),
                BirthDay = DateOnly.Parse(litter.BirthDay, _cultureInfo),
                IsActive = litter.IsActive!.Value,
                Description = litter.Description,
                MotherCat = litter.MotherCat.Decompress(),
                FatherCat = litter.FatherCat.Decompress()
            };

            foreach (var kitten in litter.Kittens)
                result.Kittens.Add(kitten.Decompress());

            return result;
        }

        #endregion

    }
}
