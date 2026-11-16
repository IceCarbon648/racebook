import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import ModCard from './index';
import { backgrounds } from '../../../assets';

const TITLE = 'Neon Skyline';
const TYPE = 'VEHICLE';
const CREATOR = 'Pat_Bonana';
const IMAGE_URL = backgrounds[0].original;

const baseProps = {
    title: TITLE,
    type: TYPE,
    imageUrl: IMAGE_URL,
    creator: CREATOR,
};

describe('ModCard', () => {
    describe('what it displays', () => {
        it('shows the title', () => {
            render(<ModCard {...baseProps} />);

            expect(screen.getByText(TITLE)).toBeInTheDocument();
        });

        it('shows the category', () => {
            render(<ModCard {...baseProps} />);

            expect(screen.getByText(TYPE)).toBeInTheDocument();
        });

        it('shows the creator with an @ prefix', () => {
            render(<ModCard {...baseProps} />);

            expect(screen.getByText(`@${CREATOR}`)).toBeInTheDocument();
        });

        it('shows the preview image labelled with the title', () => {
            render(<ModCard {...baseProps} />);

            expect(screen.getByRole('img', { name: TITLE })).toHaveAttribute('src', IMAGE_URL);
        });

        it('shows no creator line when there is no creator', () => {
            render(<ModCard {...baseProps} creator={undefined} />);

            expect(screen.queryByText(`@${CREATOR}`)).not.toBeInTheDocument();
        });
    });

    describe('opening the mod', () => {
        it('calls onClick when the card is clicked', async () => {
            const onClick = vi.fn();
            render(<ModCard {...baseProps} onClick={onClick} />);

            await userEvent.click(screen.getByText(TITLE));

            expect(onClick).toHaveBeenCalledOnce();
        });
    });

    describe('the favourite control', () => {
        it('is hidden when the mod has no favourite state', () => {
            render(<ModCard {...baseProps} onFavourite={vi.fn()} />);

            expect(screen.queryByRole('button', { name: /favourites/i })).not.toBeInTheDocument();
        });

        it('is hidden when there is no handler for it', () => {
            render(<ModCard {...baseProps} isFavourite={false} />);

            expect(screen.queryByRole('button', { name: /favourites/i })).not.toBeInTheDocument();
        });

        it('offers to add the mod when it is not favourited', () => {
            render(<ModCard {...baseProps} isFavourite={false} onFavourite={vi.fn()} />);

            expect(screen.getByRole('button', { name: 'Add to favourites' })).toBeInTheDocument();
        });

        it('offers to remove the mod when it is favourited', () => {
            render(<ModCard {...baseProps} isFavourite onFavourite={vi.fn()} />);

            expect(screen.getByRole('button', { name: 'Remove from favourites' })).toBeInTheDocument();
        });

        it('calls onFavourite when clicked', async () => {
            const onFavourite = vi.fn();
            render(<ModCard {...baseProps} isFavourite={false} onFavourite={onFavourite} />);

            await userEvent.click(screen.getByRole('button', { name: 'Add to favourites' }));

            expect(onFavourite).toHaveBeenCalledOnce();
        });

        it('does not open the mod when it is clicked', async () => {
            const onClick = vi.fn();
            render(
                <ModCard
                    {...baseProps}
                    isFavourite={false}
                    onFavourite={vi.fn()}
                    onClick={onClick}
                />
            );

            await userEvent.click(screen.getByRole('button', { name: 'Add to favourites' }));

            expect(onClick).not.toHaveBeenCalled();
        });
    });

    describe('the owner controls', () => {
        it('are hidden when neither handler is supplied', () => {
            render(<ModCard {...baseProps} />);

            expect(screen.queryByRole('button', { name: 'Edit mod' })).not.toBeInTheDocument();
            expect(screen.queryByRole('button', { name: 'Delete mod' })).not.toBeInTheDocument();
        });

        it('are hidden when only one handler is supplied', () => {
            render(<ModCard {...baseProps} onEdit={vi.fn()} />);

            expect(screen.queryByRole('button', { name: 'Edit mod' })).not.toBeInTheDocument();
        });

        it('are shown when both handlers are supplied', () => {
            render(<ModCard {...baseProps} onEdit={vi.fn()} onDelete={vi.fn()} />);

            expect(screen.getByRole('button', { name: 'Edit mod' })).toBeInTheDocument();
            expect(screen.getByRole('button', { name: 'Delete mod' })).toBeInTheDocument();
        });

        it('calls onEdit when edit is clicked', async () => {
            const onEdit = vi.fn();
            render(<ModCard {...baseProps} onEdit={onEdit} onDelete={vi.fn()} />);

            await userEvent.click(screen.getByRole('button', { name: 'Edit mod' }));

            expect(onEdit).toHaveBeenCalledOnce();
        });

        it('calls onDelete when delete is clicked', async () => {
            const onDelete = vi.fn();
            render(<ModCard {...baseProps} onEdit={vi.fn()} onDelete={onDelete} />);

            await userEvent.click(screen.getByRole('button', { name: 'Delete mod' }));

            expect(onDelete).toHaveBeenCalledOnce();
        });

        it('does not open the mod when edit is clicked', async () => {
            const onClick = vi.fn();
            render(
                <ModCard
                    {...baseProps}
                    onClick={onClick}
                    onEdit={vi.fn()}
                    onDelete={vi.fn()}
                />
            );

            await userEvent.click(screen.getByRole('button', { name: 'Edit mod' }));

            expect(onClick).not.toHaveBeenCalled();
        });

        it('does not open the mod when delete is clicked', async () => {
            const onClick = vi.fn();
            render(
                <ModCard
                    {...baseProps}
                    onClick={onClick}
                    onEdit={vi.fn()}
                    onDelete={vi.fn()}
                />
            );

            await userEvent.click(screen.getByRole('button', { name: 'Delete mod' }));

            expect(onClick).not.toHaveBeenCalled();
        });
    });
});
