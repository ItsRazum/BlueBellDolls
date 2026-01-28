export const getKittenPhotoUrl = (kitten: Kitten): string => {

    if ('mainPhotoUrl' in kitten) {
        return `${kitten.mainPhotoUrl}`;
    }

    if ('photos' in kitten) {
        return `${kitten.photos[0].url}`;
    }

    return '';
}