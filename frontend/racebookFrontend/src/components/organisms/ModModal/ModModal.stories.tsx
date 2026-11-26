import type { Meta, StoryObj } from '@storybook/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import ModModal from './index';
import { queryKeys } from '../../../constants/queryKeys';
import type { MyMod } from '../../../types';

const CATEGORIES = ['ENVIRONMENT', 'VEHICLE', 'PACK', 'UI', 'MISC'];

const EXISTING_MOD = {
    modId: '22222222-2222-2222-2222-222222222222',
    title: 'Midnight Expressway',
    type: 'ENVIRONMENT',
    description: 'A resurfaced version of the motorway stage, with new barriers and lighting.',
    modFileUrl: 'https://res.cloudinary.com/racebook/raw/upload/v1/Mods/o04a2toyr9v2weqm3tqv.tpf',
    imageUrl: 'https://res.cloudinary.com/racebook/image/upload/v1/PreviewImages/sbhal99jwoe39i9sejoz.png',
} as unknown as MyMod;

const withCategories = (Story: () => React.ReactElement) => {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false, staleTime: Infinity } },
    });

    queryClient.setQueryData(queryKeys.categories, CATEGORIES);

    return (
        <QueryClientProvider client={queryClient}>
            <Story />
        </QueryClientProvider>
    );
};

const meta = {
    title: 'Organisms/ModModal',
    component: ModModal,
    parameters: {
        layout: 'fullscreen'
    },
    decorators: [withCategories],
    args: {
        onClose: () => { },
        onSubmit: async () => { }
    }
} satisfies Meta<typeof ModModal>;

export default meta;

type Story = StoryObj<typeof meta>;

export const Upload: Story = {
    args: {
        mode: 'upload',
    },
};

export const Edit: Story = {
    args: {
        mode: 'edit',
        mod: EXISTING_MOD,
    },
};

export const Submitting: Story = {
    args: {
        mode: 'edit',
        mod: EXISTING_MOD,
        onSubmit: () => new Promise((resolve) => setTimeout(resolve, 2500)),
    },
};

export const LongContent: Story = {
    args: {
        mode: 'edit',
        mod: {
            ...EXISTING_MOD,
            title: 'Midnight Expressway Resurfaced',
            description:
                'A full resurface of the motorway stage with new barriers, revised lighting, repainted road markings and additional roadside detail throughout the tunnel section.',
        } as unknown as MyMod,
    },
};