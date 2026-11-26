import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../constants/queryKeys";
import { getModDetails } from "../../services";

export const useModDetails = (modId: string) => {
  const modDetails = useQuery({
    queryKey: queryKeys.modDetails(modId),
    queryFn: () => getModDetails(modId),
  });

  return modDetails;
};
