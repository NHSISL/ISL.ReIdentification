import { useQuery } from '@tanstack/react-query';
import EntraUsersBroker from '../../brokers/apiBroker.entra.users';

export const entraUsersService = {
    useSearchEntraUsers: (searchTerm : string) => {
        const entraUsersBroker = new EntraUsersBroker();
        console.log(searchTerm)
        return useQuery({
            queryKey: ["EntraUsersSearch", { query: searchTerm }],
            queryFn: async () => await entraUsersBroker.FilterUsersAsync(searchTerm),
            staleTime: Infinity
        });
    }
};