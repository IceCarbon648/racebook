import { useQuery } from "@tanstack/react-query";
import { getMyMods } from "../../../services";
import { queryKeys } from "../../../constants/queryKeys";

export const useMyMods = () => {
  const myMods = useQuery({
    queryKey: queryKeys.myMods,
    queryFn: getMyMods,
  });

  return myMods;
};
