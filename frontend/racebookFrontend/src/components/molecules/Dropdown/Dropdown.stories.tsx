import { useState } from 'react';
import type { Meta, StoryObj } from '@storybook/react';
import Dropdown from './index';

const CATEGORIES = ['ALL TYPES', 'ENVIRONMENT', 'VEHICLE', 'PACK', 'UI', 'MISC'];

const meta = {
    title: 'Molecules/Dropdown',
    component: Dropdown,
    decorators: [
        (Story) => (
            <div style={{ background: '#0b0b12', padding: '4rem', width: '20rem' }}>
                <Story />
            </div>
        ),
    ],
    args: {
        options: CATEGORIES,
        label: 'Filter by category',
        onChange: () => {},
    },
} satisfies Meta<typeof Dropdown>;

export default meta;

type Story = StoryObj<typeof meta>;

export const Interactive: Story = {
    args: {
        value: 'ALL TYPES',
    },
    render: function Render(args) {
        const [value, setValue] = useState(args.value);

        return <Dropdown {...args} value={value} onChange={setValue} />;
    },
};

export const NothingSelected: Story = {
    args: {
        value: '',
        options: CATEGORIES.slice(1),
        label: 'Select a category',
    },
};

export const TwoOptions: Story = {
    args: {
        value: 'Newest first',
        options: ['Newest first', 'Oldest first'],
        label: 'Sort order',
    },
};

export const LongOption: Story = {
    args: {
        value: 'ENVIRONMENT',
        options: ['ENVIRONMENT', 'A VERY LONG CATEGORY NAME INDEED'],
    },
};