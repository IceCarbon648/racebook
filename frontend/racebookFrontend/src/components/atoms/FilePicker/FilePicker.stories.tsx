import { useState } from 'react';
import type { Meta, StoryObj } from '@storybook/react';
import FilePicker from './index';

const CLOUDINARY_NAME = 'o04a2toyr9v2weqm3tqv.tpf';

const makeFile = (name: string) => new File(['x'], name);

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
        file: null,
        existingFileName: null,
        onChange: () => {},
    },
} satisfies Meta<typeof FilePicker>;

export default meta;

type Story = StoryObj<typeof meta>;

/** How the picker appears on the upload form, before anything is chosen. */
export const Empty: Story = {};

/** How it appears on the edit form, showing the file already stored. */
export const WithExistingFile: Story = {
    args: {
        existingFileName: CLOUDINARY_NAME,
    },
};

/** Once a new file is picked, its name replaces the stored one. */
export const WithSelectedFile: Story = {
    args: {
        file: makeFile('neon-skyline.tpf'),
        existingFileName: CLOUDINARY_NAME,
    },
};

/** The preview image variant, which accepts a different set of extensions. */
export const ImagePicker: Story = {
    args: {
        id: 'previewImage',
        label: 'Preview Image',
        accept: '.png,.jpg,.jpeg',
        existingFileName: 'sbhal99jwoe39i9sejoz.png',
    },
};

/** A long name, to check the truncation and the hover title. */
export const LongFileName: Story = {
    args: {
        file: makeFile('midnight-expressway-resurfaced-final-v3.tpf'),
    },
};

/** Picks up the real file you choose, so the display logic can be tried out. */
export const Interactive: Story = {
    render: function Render(args) {
        const [file, setFile] = useState<File | null>(null);

        return <FilePicker {...args} file={file} onChange={setFile} />;
    },
};
