import React, { useState, useEffect } from "react";

const UpdatableImage = ({ src, onImageUpload, className }) => {
    const [imageSrc, setImageSrc] = useState(src); // Local state for the image
    const [isUploading, setIsUploading] = useState(false);

    useEffect(() => {
        setImageSrc(src);
    }, [src]);

    const handleFileChange = async (event) => {
        const file = event.target.files[0];
        if (!file) return;

        if (onImageUpload) {
            setIsUploading(true);
            await onImageUpload(file);
            setIsUploading(false);
        }
    };

    return (
        <div style={{ position: "relative", textAlign: "center" }}>
            <label htmlFor="image-upload" style={{ cursor: "pointer" }}>
                <img
                    src={imageSrc}
                    alt="Updatable"
                    className={className}
                    style={{
                        opacity: isUploading ? 0.5 : 1,
                        transition: "opacity 0.3s",
                    }}
                />
                {isUploading && (
                    <div
                        style={{
                            position: "absolute",
                            top: "50%",
                            left: "50%",
                            transform: "translate(-50%, -50%)",
                            fontSize: "16px",
                            fontWeight: "bold",
                            color: "#fff",
                            backgroundColor: "rgba(0, 0, 0, 0.5)",
                            padding: "10px",
                            borderRadius: "8px",
                        }}
                    >
                        Uploading...
                    </div>
                )}
            </label>
            <input
                id="image-upload"
                type="file"
                accept="image/*"
                style={{ display: "none" }}
                onChange={handleFileChange}
            />
        </div>
    );
};

export default UpdatableImage;
