import type { Meta, StoryObj } from '@storybook/react';
import FilePicker from './index';

const CLOUDINARY_NAME = 'o04a2toyr9v2weqm3tqv.tpf';

const meta = {
    title: 'Atoms/FilePicker',
    component: FilePicker,
    decorators: [
        (Story) => (
            <div style={{ background: '#0b0b12', padding: '4rem', width: '16rem' }}>
                <Story />
            </div>
        ),
    ],
    args: {
        id: 'modFile',
        label: 'Mod File',
        accept: '.tpf',
        existingFileName: null,
    },
} satisfies Meta<typeof FilePicker>;

export default meta;

type Story = StoryObj<typeof meta>;

export const Empty: Story = {};

export const WithExistingFile: Story = {
    args: {
        existingFileName: CLOUDINARY_NAME,
    },
};

export const ImagePicker: Story = {
    args: {
        id: 'previewImage',
        label: 'Preview Image',
        accept: '.png,.jpg,.jpeg',
        existingFileName: 'sbhal99jwoe39i9sejoz.png',
    },
};

export const LongFileName: Story = {
    args: {
        existingFileName: 'midnight-expressway-resurfaced-final-v3.tpf',
    },
};