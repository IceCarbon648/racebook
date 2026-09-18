import { FORM_SHAPE, FORM_ACCENT_TOP, FORM_ACCENT_BOTTOM } from '../../../constants/customDivs';
import { ACCENT } from '../../../constants/theme';
import type { FormPanelProps } from './index.types';

const FormPanel = ({ children, className = '' }: FormPanelProps) => {
    return (
        <div className={`relative isolate w-[512px] max-w-sm h-150`}>
            <div
                className="pointer-events-none absolute inset-0 -z-10 backdrop-blur-[3px]"
                style={{
                    clipPath: `path('${FORM_SHAPE}')`,
                    WebkitClipPath: `path('${FORM_SHAPE}')`,
                }}
            />

            <svg
                className="pointer-events-none absolute inset-0 -z-10 h-full w-full"
                viewBox="0 0 512 800"
                preserveAspectRatio="none"
            >
                <path d={FORM_SHAPE} fill="#000000" fillOpacity={0.5} />
                <path d={FORM_ACCENT_TOP} fill={ACCENT} />
                <path d={FORM_ACCENT_BOTTOM} fill={ACCENT} />
            </svg>

            <div className="absolute inset-x-0 top-7 bottom-7 overflow-y-auto px-10 [scrollbar-width:none] [&::-webkit-scrollbar]:hidden">
                <div className={`${className}`}>
                    {children}
                </div>
            </div>
        </div>
    );
};

export default FormPanel;