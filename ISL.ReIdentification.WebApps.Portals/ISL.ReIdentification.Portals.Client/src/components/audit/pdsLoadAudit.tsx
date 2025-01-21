import { FunctionComponent, useState } from 'react';
import { auditViewService } from '../../services/views/audit/auditViewService';
import moment from 'moment';
import { Alert, OverlayTrigger, Tooltip } from 'react-bootstrap';

interface PdsLoadAuditProps {
    isAlert?: boolean;
    isHover?: boolean;
}

export const PdsLoadAudit: FunctionComponent<PdsLoadAuditProps> = ({ isAlert = false, isHover = false }) => {
    const { data, isLoading } = auditViewService.useGetPdsAuditByAuditType("PdsDataLoad");
    const [showTooltip, setShowTooltip] = useState(false);

    if (isLoading || !data || !data?.createdDate) {
        return <></>;
    }

    const content = `PDS last uploaded: ${moment(data.createdDate.toString()).format("DD/MM/YY HH:mm")}`;
    const variant = data.auditDetail === 'failure' ? 'danger' : data.auditDetail;

    if (isAlert) {
        return <Alert variant={variant} className="m-0 p-1">{content}</Alert>;
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
                    PDS
                </div>
            </OverlayTrigger>
        );
    }

    return <>{content}</>;
};