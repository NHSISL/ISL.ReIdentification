import { FunctionComponent, useState } from 'react';
import { auditViewService } from '../../services/views/audit/auditViewService';
import moment from 'moment';
import { Alert, OverlayTrigger, Tooltip } from 'react-bootstrap';

interface OdsLoadAuditProps {
    isAlert?: boolean;
    isHover?: boolean;
}

export const OdsLoadAudit: FunctionComponent<OdsLoadAuditProps> = ({ isAlert = false, isHover = false }) => {
    const { data, isLoading } = auditViewService.useGetOdsAudit();
    const [showTooltip, setShowTooltip] = useState(false);

    if (isLoading || !data || !data[0]?.createdDate) {
        return <></>;
    }

    const content = `ODS last uploaded: ${moment(data[0].createdDate.toString()).format("YY/MM/DD HH:mm")}`;
    const variant = data[0].auditDetail === 'failure' ? 'danger' : data[0].auditDetail;

    if (isAlert) {
        return <Alert variant={variant} className="m-0 mb-2">{content}</Alert>;
    }

    if (isHover) {
        return (
            <OverlayTrigger
                placement="top"
                overlay={<Tooltip id="tooltip-top">{content}</Tooltip>}
                show={showTooltip}
                onToggle={() => setShowTooltip(!showTooltip)}
            >
                <div
                    style={{
                        width: '50px',
                        height: '50px',
                        borderRadius: '50%',
                        backgroundColor: '#007bff',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        color: 'white',
                        fontSize: '18px',
                        cursor: 'pointer'
                    }}
                    onMouseEnter={() => setShowTooltip(true)}
                    onMouseLeave={() => setShowTooltip(false)}
                >
                    ODS
                </div>
            </OverlayTrigger>
        );
    }

    return <>{content}</>;
};