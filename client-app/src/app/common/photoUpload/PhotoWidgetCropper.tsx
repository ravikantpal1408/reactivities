import React, {useRef} from 'react';
import Cropper from 'react-cropper';
import 'cropperjs/dist/cropper.css';

interface IProps {
    setImage: (file: Blob) => void;
    imagePreview: string;
}

const PhotoWidgetCropper: React.FC<IProps> = ({setImage, imagePreview}) => {
    
    const cropper = useRef<Cropper>(null);

    const cropImage = () => {
        if (
            cropper.current &&
            typeof cropper.current.getCroppedCanvas() === 'undefined'
        ) {
            return;
        }
        cropper &&
        cropper.current &&
        cropper.current.getCroppedCanvas().toBlob((blob: any) => {
            setImage(blob);
        }, 'image/jpeg');
    };


    
    return (
        <div>
            <Cropper
                ref={cropper}
                src={imagePreview}
                style={{height: 400, width: '100%'}}
                // Cropper.js options
                preview='.img-preview'
                aspectRatio={1 / 1}
                guides={false}
                viewMode={1}
                scalable={true}
                crop={cropImage}
            />
        </div>
    )
};

export default PhotoWidgetCropper;