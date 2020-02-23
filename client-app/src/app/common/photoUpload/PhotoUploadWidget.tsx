import React, { Fragment, useEffect, useState } from 'react';
import { observer } from "mobx-react-lite";
import { Grid, Header, Button } from "semantic-ui-react";
import PhotoWidgetDropzone from "./PhotoWidgetDropzone";
import PhotoWidgetCropper from "./PhotoWidgetCropper";

interface IProps {
    loading: boolean;
    uploadPhoto: (file: Blob) => void;
}

const PhotoUploadWidget: React.FC<IProps> = ({ loading, uploadPhoto }) => {
    const [files, setFiles] = useState<any[]>([]);
    const [image, setImage] = useState<Blob | null>(null);

    useEffect(() => {
        return () => {
            files.forEach(file => URL.revokeObjectURL(file.preview));
        }
    });
    return (
        <Fragment>
            <Grid>
                <Grid.Column width={4}>
                    <Header color='teal' content='Step 1 - Add Photo' sub />
                    <PhotoWidgetDropzone setFiles={setFiles} />
                </Grid.Column>

                <Grid.Column width={1} />

                <Grid.Column width={4}>
                    <Header color='teal' content='Step 2 - Resize Photo' sub />
                    {files.length > 0 && <PhotoWidgetCropper setImage={setImage} imagePreview={files[0].preview} />}
                </Grid.Column>

                <Grid.Column width={1} />

                <Grid.Column width={4}>
                    <Header color='teal' content='Step 3 - Preview & Upload' sub />
                    {files.length > 0 &&
                        <Fragment>
                            <div className="img-preview" style={{ minHeight: '200px', overflow: 'hidden' }} />
                            <Button.Group widths={2}>
                                <Button positive icon='check' loading={loading} onClick={() => uploadPhoto(image!)} />
                                <Button icon='close' disabled={loading} onClick={() => setFiles([])} />
                            </Button.Group>
                        </Fragment>

                    }
                </Grid.Column>

            </Grid>
        </Fragment>
    );
};

export default observer(PhotoUploadWidget);