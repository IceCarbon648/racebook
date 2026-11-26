import { notify } from "../utils/notify";
import { useFavouriteMod } from "./api/useFavouriteMod";

export const useFavouriteClick = () => {
  const { toggleFavourite } = useFavouriteMod();

  const handleFavouriteClick = async (modId: string, isFavourite: boolean) => {
    try {
      await toggleFavourite(modId, isFavourite);

      notify.success(
        isFavourite ? "Removed from favourites" : "Added to favourites",
      );
    } catch (err) {
      notify.apiError(
        err,
        "Could not update your favourites, please try again",
      );
    }
  };

  return { handleFavouriteClick };
};
