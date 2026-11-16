import type { Meta, StoryObj } from '@storybook/react';
import ModCard from './index';
import { backgrounds } from '../../../assets';

const PREVIEW_IMAGE = backgrounds[0].original;

const meta = {
    title: 'Molecules/ModCard',
    component: ModCard,
    decorators: [
        (Story) => (
            <div style={{ background: '#0b0b12', padding: '4rem', display: 'inline-block' }}>
                <Story />
            </div>
        ),
    ],
    args: {
        title: 'Banan the mod',
        type: 'VEHICLE',
        imageUrl: PREVIEW_IMAGE,
        creator: 'Pat_Bonana',
    },
} satisfies Meta<typeof ModCard>;

export default meta;

type Story = StoryObj<typeof meta>;

export const Default: Story = {
    args: {
        onClick: () => {},
    },
};

export const NotFavourited: Story = {
    args: {
        isFavourite: false,
        onClick: () => {},
        onFavourite: () => {},
    },
};

export const Favourited: Story = {
    args: {
        isFavourite: true,
        onClick: () => {},
        onFavourite: () => {},
    },
};

export const Owned: Story = {
    args: {
        creator: undefined,
        onClick: () => {},
        onEdit: () => {},
        onDelete: () => {},
    },
};

export const LongTitle: Story = {
    args: {
        title: 'Banan the super long name',
        onClick: () => {},
    },
};